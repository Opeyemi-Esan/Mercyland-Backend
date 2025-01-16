using MercylandAdmin.Interface;
using MercylandAdmin.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace MercylandAdmin.Inplementation
{
    public class VideoAdvertService : IVideoAdvertService
    {
        private readonly AppDbContext _appDbContext;
        private readonly string _uploadFolder;

        public VideoAdvertService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "videoUploads");
            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }
        public async Task<string> AddVideoAdvert(VideoAdvertDTO videoAdvertDTO)
        {
            if (videoAdvertDTO == null) 
            {
                throw new ArgumentNullException(nameof(videoAdvertDTO), "data transfer object cannot be null.");
            }

            string filePath = null;

            //handle file upload
            if (videoAdvertDTO != null && videoAdvertDTO.VideoFile.Length > 0)
            {
                var allowedExtension = new[] { ".mp4" };
                var fileExtension = Path.GetExtension(videoAdvertDTO.VideoFile.FileName).ToLower();

                //check if file extension is valid
                if (!allowedExtension.Contains(fileExtension)) 
                {
                    throw new Exception("File uploaded not valid, upload only mp4 file");
                }

                try
                {
                    var videoAdFile = Path.GetFileName(videoAdvertDTO.VideoFile.FileName);
                    filePath = Path.Combine(_uploadFolder, videoAdFile);

                    //check file existence to handle conflits
                    if (System.IO.File.Exists(filePath))
                    {
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(videoAdFile);
                        var uniqueFileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}{fileExtension}";
                        filePath = Path.Combine(_uploadFolder, uniqueFileName);
                    }

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await videoAdvertDTO.VideoFile.CopyToAsync(stream);
                }
                catch (Exception ex) 
                {
                    throw new Exception("File upload error", ex);
                }
            }

            var videoupload = new VideosAdvert
            {
                VideoFile = filePath
            };
            _appDbContext.VideosAdverts.Add(videoupload);
            await _appDbContext.SaveChangesAsync();
            return "File uploaded successfully";
        }

        public async Task<VideosAdvert> GetVideoAdvert()
        {
            var response = await _appDbContext.VideosAdverts.ToListAsync();
            return response.FirstOrDefault();
        }

        public async Task<string> UpdateVideoAdvert(int id, VideoAdvertDTO videoAdvertDTO)
        {
            var videoAdsUpdate = await _appDbContext.VideosAdverts.FirstOrDefaultAsync(x => x.Id == id);
                if (videoAdsUpdate == null)
                {
                    throw new KeyNotFoundException($"Property with id {id} not found.");
                }


                // Handle file upload  
                if (videoAdvertDTO.VideoFile != null && videoAdvertDTO.VideoFile.Length > 0)
                {
                    var propertyImage = Path.GetFileName(videoAdvertDTO.VideoFile.FileName);
                    var filePath = Path.Combine(_uploadFolder, propertyImage);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await videoAdvertDTO.VideoFile.CopyToAsync(stream);

                   
                    videoAdsUpdate.VideoFile = filePath;

                }
                await _appDbContext.SaveChangesAsync();
            return "Video updated successfully";
            
        }
    }
}
