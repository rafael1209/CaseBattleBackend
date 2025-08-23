using CaseBattleBackend.Dtos;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class BranchService(IBranchRepository branchRepository, IStorageService storageService)
    : IBranchService
{
    public async Task<List<BranchView>> GetAllBranchesAsync()
    {
        var branches = await branchRepository.GetAllBranchesAsync();

        var branchViews = new List<BranchView>();
        foreach (var branch in branches)
        {
            var imageUrls = new List<Uri>();
            foreach (var imageUrl in branch.ImageIds)
                imageUrls.Add(await storageService.GetFileUrl(imageUrl));

            branchViews.Add(new BranchView
            {
                Id = branch.Id.ToString(),
                Name = branch.Name,
                Description = branch.Description,
                Coordinates = branch.Coordinates,
                ImageUrls = imageUrls
            });
        }

        return branchViews;
    }

    public async Task<Branch?> GetBranchByIdAsync(ObjectId branchId)
    {
        return await branchRepository.GetBranchByIdAsync(branchId);
    }

    public async Task<BranchView?> GetBranchViewById(ObjectId branchId)
    {
        var branch = await branchRepository.GetBranchByIdAsync(branchId);
        if (branch == null)
            return null;
        var imageUrls = new List<Uri>();
        foreach (var imageId in branch.ImageIds)
            imageUrls.Add(await storageService.GetFileUrl(imageId));
        return new BranchView
        {
            Id = branch.Id.ToString(),
            Name = branch.Name,
            Description = branch.Description,
            Coordinates = branch.Coordinates,
            ImageUrls = imageUrls
        };
    }

    public async Task<Branch> CreateBranchAsync(CreateBranchRequest request)
    {
        var imageIds = new List<string>();
        if (request.Files != null)
        {
            foreach (var file in request.Files)
            {
                var imageDto = await storageService.UploadFile(file, file.Name);

                imageIds.Add(imageDto.Id);
            }
        }

        var branch = new Branch
        {
            Name = request.Name,
            Description = request.Description,
            Coordinates = request.Coordinates,
            ImageIds = imageIds,
        };

        return await branchRepository.CreateBranchAsync(branch);
    }

    public Task<bool> UpdateBranchAsync(Branch branch)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteBranchAsync(ObjectId branchId)
    {
        throw new NotImplementedException();
    }
}