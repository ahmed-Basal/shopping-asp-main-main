using core.Dto;
using core.Entities;
using core.Services;
using inftastructer.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace inftastructer.Repository.Services
{
    public  class CommentService: ICommentService
    {
        private readonly AppDbContext _context;

        public CommentService(AppDbContext context)
        {
            _context = context;
        }

        // Add Comment or Reply
        public async Task<comment> AddCommentAsync(AddCommentDto dto, string userId)
        {
            var comment = new comment
            {
                Content = dto.Content,
                ProductId = dto.ProductId,
                UserId = userId,
                ParentCommentId = dto.ParentCommentId
            };

            _context.Comment.Add(comment);

            await _context.SaveChangesAsync();

            return comment;
        }

        // Get Product Comments
        public async Task<List<comment>> GetProductComments(int productId)
        {
            return await _context.Comment
                .Where(x => x.ProductId == productId && x.ParentCommentId == null)
                .Include(x => x.User)
                .Include(x => x.Replies)
                .ToListAsync();
        }

        // Edit Comment
        public async Task<comment?> EditComment(int commentId, string newContent, string userId)
        {
            var comment = await _context.Comment
                .FirstOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
                return null;

            // تأكد أن نفس المستخدم
            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("You cannot edit this comment");

            comment.Content = newContent;

            await _context.SaveChangesAsync();

            return comment;
        }

        // Delete Comment
        public async Task<bool> DeleteComment(int commentId, string userId)
        {
            var comment = await _context.Comment
                .FirstOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
                return false;

            // تأكد أن نفس المستخدم
            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("You cannot delete this comment");

            _context.Comment.Remove(comment);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
