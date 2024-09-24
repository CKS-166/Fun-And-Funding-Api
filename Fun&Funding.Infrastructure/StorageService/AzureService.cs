using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Fun_Funding.Application.IStorageService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Infrastructure.StorageService
{
    public class AzureService : IAzureService
    {
        BlobServiceClient _blobServiceClient;
        BlobContainerClient _blobContainerClient;
        private string cnContainer = "DefaultEndpointsProtocol=https;AccountName=funfundingmediafiles;AccountKey=Qr/V2+T1qTPRU85rvsQecbfIXCN9k9rPieCb0nK5R8SZvi1bqGAv85kp5IaFlAFjDhNHUB7WxOh++ASt9KE9LQ==;EndpointSuffix=core.windows.net";
        public AzureService() {
            _blobServiceClient = new BlobServiceClient(cnContainer);
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient("fundingprojectfiles");
        }
        public async Task<List<BlobContentInfo>> UploadBlobFiles(List<IFormFile> files)
        {
            var azureRes = new List<BlobContentInfo>();
            foreach (var file in files)
            {
                string fileName = file.FileName;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var client = await _blobContainerClient.UploadBlobAsync(fileName, memoryStream);
                    azureRes.Add(client);
                }
            }
            return azureRes;
        }
        public async Task<List<BlobContentInfo>> UploadFiles(List<IFormFile> files)
        {
            var azureRes = new List<BlobContentInfo>();
            foreach (var file in files)
            {
                string fileName = file.FileName;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    // Tạo BlobClient từ container client
                    var blobClient = _blobContainerClient.GetBlobClient(fileName);

                    // Thiết lập HTTP Headers với Content-Type phù hợp
                    var blobHttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType // Lấy Content-Type từ file upload
                    };

                    // Tải file lên blob với Content-Type đã đặt
                    var response = await blobClient.UploadAsync(memoryStream, new BlobUploadOptions
                    {
                        HttpHeaders = blobHttpHeaders
                    });

                    azureRes.Add(response.Value);
                }
            }
            return azureRes;
        }



        public async Task<List<BlobItem>> GetUploadedItems()
        {
            var items = new List<BlobItem>();
            var azureRes = _blobContainerClient.GetBlobsAsync();
            await foreach (var item in azureRes)
            {
                items.Add(item);
            }
            return items;

        }

        public List<string> UploadUrlBlobFiles(List<IFormFile> files)
        {
            var uploadedUrls = new List<string>();
            foreach (var file in files)
            {
                string fileName = file.FileName;
                string uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                using (var memoryStream = new MemoryStream())
                {
                     file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    // Tạo BlobClient cho từng file
                    var blobClient = _blobContainerClient.GetBlobClient(uniqueFileName);
                    // Thiết lập HTTP Headers với Content-Type phù hợp
                    var blobHttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType // Lấy Content-Type từ file upload
                    };

                    // Upload file lên Blob Storage
                     blobClient.UploadAsync(memoryStream, new BlobUploadOptions
                    {
                        HttpHeaders = blobHttpHeaders
                    });

                    // Lấy URL của file sau khi upload
                    string fileUrl = blobClient.Uri.ToString();

                    // Thêm URL vào danh sách kết quả trả về
                    uploadedUrls.Add(fileUrl);
                }
            }
            return uploadedUrls;
        }
    }
}
