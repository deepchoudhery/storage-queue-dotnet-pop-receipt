//----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//----------------------------------------------------------------------------------

namespace PopreceiptSample
{
    using Azure;
    using Azure.AI.Vision.Face;
    using Azure.Data.Tables;
    using Azure.Storage.Blobs;
    using Azure.Storage.Queues;
    using Azure.Storage.Queues.Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Demonstrates using a queue message pop receipt to coordinate updates across
    /// blob and table storage.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Azure Storage Queue Sample demonstrating popreceipt functionality\n");

            MainAsync(args).GetAwaiter().GetResult();

            Console.WriteLine("Press any key to exit");
            Console.Read();
        }

        private static async Task MainAsync(string[] args)
        {
            QueueClient queue = await CreateQueueAsync();
            BlobContainerClient container = await CreateBlobContainerAsync();
            TableClient table = await CreateTableAsync();

            string faceEndpoint = ConfigurationManager.AppSettings["FaceAPIEndpoint"];
            string faceApiKey = ConfigurationManager.AppSettings["FaceAPIKey"];
            var faceClient = new FaceClient(new Uri(faceEndpoint), new AzureKeyCredential(faceApiKey));

            try
            {
                await ProcessImages(queue, container, table, faceClient);
            }
            finally
            {
                await Cleanup(queue, container, table);
            }
        }

        private static async Task ProcessImages(
            QueueClient queue,
            BlobContainerClient container,
            TableClient table,
            FaceClient faceClient)
        {
            try
            {
                IEnumerable<string> images = Directory.EnumerateFiles("testfolder", "*.jpg");

                foreach (string currentFile in images)
                {
                    string fileName = Path.GetFileName(currentFile);

                    Console.WriteLine("Processing image {0}", fileName);

                    Response<SendReceipt> sendResponse = await queue.SendMessageAsync(
                        fileName,
                        visibilityTimeout: TimeSpan.FromSeconds(900));
                    SendReceipt sendReceipt = sendResponse.Value;

                    using (FileStream fileStream = File.OpenRead(currentFile))
                    {
                        IReadOnlyList<FaceDetectionResult> faces;
                        try
                        {
                            Response<IReadOnlyList<FaceDetectionResult>> detectResponse =
                                await faceClient.DetectAsync(
                                    BinaryData.FromStream(fileStream),
                                    FaceDetectionModel.Detection01,
                                    FaceRecognitionModel.Recognition04,
                                    returnFaceId: false,
                                    returnFaceAttributes: new[] { FaceAttributeType.Age },
                                    returnFaceLandmarks: true);
                            faces = detectResponse.Value;
                        }
                        catch (RequestFailedException ex)
                        {
                            Console.WriteLine("Face API error: " + ex.ErrorCode + ex.Message);
                            return;
                        }

                        Console.WriteLine(faces.Count + " face(s) detected in " + fileName);

                        BlobClient blob = container.GetBlobClient(fileName);
                        var tableEntity = new TableEntity("FaceImages", fileName);

                        int i = 1;
                        foreach (FaceDetectionResult face in faces)
                        {
                            tableEntity.Add(
                                "person" + i,
                                face.FaceAttributes.Age?.ToString());
                            i++;

                            if (i > 250)
                            {
                                break;
                            }
                        }

                        if (faces.Count > 0)
                        {
                            await blob.UploadAsync(currentFile, overwrite: true);
                        }

                        await table.UpsertEntityAsync(tableEntity, TableUpdateMode.Replace);

                        bool blobExists = (await blob.ExistsAsync()).Value;
                        NullableResponse<TableEntity> storedEntity =
                            await table.GetEntityIfExistsAsync<TableEntity>("FaceImages", fileName);

                        if (blobExists && storedEntity.HasValue)
                        {
                            await queue.DeleteMessageAsync(
                                sendReceipt.MessageId,
                                sendReceipt.PopReceipt);
                        }
                    }
                }
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("Storage error: " + ex.ErrorCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);

                if (ex is DirectoryNotFoundException || ex is FileNotFoundException)
                {
                    Console.WriteLine(
                        "Please make sure that the folder \"testfolder\" (with images) is present in the current directory where the sample is running");
                }
            }
        }

        private static async Task<BlobContainerClient> CreateBlobContainerAsync()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var container = new BlobContainerClient(connectionString, "samplecontainer");

            Console.WriteLine("Creating a container for the demo");
            try
            {
                await container.CreateIfNotExistsAsync();
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("Error: " + ex.ErrorCode + ex.Message);
                throw;
            }

            return container;
        }

        private static async Task<QueueClient> CreateQueueAsync()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var options = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };
            var queue = new QueueClient(connectionString, "samplequeue", options);

            Console.WriteLine("Creating a queue for the demo");
            try
            {
                await queue.CreateIfNotExistsAsync();
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("Error: " + ex.ErrorCode + ex.Message);
                throw;
            }

            return queue;
        }

        private static async Task<TableClient> CreateTableAsync()
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var table = new TableClient(connectionString, "sampletable");

            Console.WriteLine("Creating a table for the demo");
            try
            {
                await table.CreateIfNotExistsAsync();
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("Error: " + ex.ErrorCode + ex.Message);
                throw;
            }

            return table;
        }

        private static async Task Cleanup(
            QueueClient queue,
            BlobContainerClient container,
            TableClient table)
        {
            Console.WriteLine("Cleaning up the queue, table and the blobs created");

            try
            {
                await queue.DeleteIfExistsAsync();
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("Storage error: " + ex.ErrorCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            try
            {
                await container.DeleteIfExistsAsync();
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("Storage error: " + ex.ErrorCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            try
            {
                await table.DeleteAsync();
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("Storage error: " + ex.ErrorCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}
