using core.Dto;
using core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.Services
{
    public  interface ICommentService
    {
        Task<comment> AddCommentAsync(AddCommentDto dto, string userId);

        Task<List<comment>> GetProductComments(int productId);

        Task<comment?> EditComment(int commentId, string newContent, string userId);

        Task<bool> DeleteComment(int commentId, string userId);
    }
}
