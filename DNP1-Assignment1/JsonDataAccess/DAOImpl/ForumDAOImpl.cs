﻿
using Application;
using Entities.Models;

namespace JsonDataAccess;

public class ForumDAOImpl:IForumDAO

{
    private JsonForumContext fileContext;

    public ForumDAOImpl(JsonForumContext FileContext) {
        this.fileContext = FileContext;
    }

    
    
    
    public async Task<Forum> AddForumAsync(Forum newForumItem) {
        //  Console.WriteLine("ForumDAO");
        if (fileContext.Forums.Any()) {
            int largestId = fileContext.Forums.Max(forum => forum.Id);
            newForumItem.Id = largestId + 1;
        }
        else {
            newForumItem.Id = 1;
        }

        fileContext.Forums.Add(newForumItem);
        await fileContext.SaveChangesAsync();
        return newForumItem;
    }

    public async Task<Forum> GetForumByIdAsync(int id) {
        return fileContext.Forums.First(forum => forum.Id == id);
    }
    
    
    public async Task<List<Forum>> GetAllForumsAsync() {
        return fileContext.Forums.ToList();
    }
    
    public async Task<SubForum> GetSubForumAsync(int subForumId)
    {

        return fileContext.Forums.First().AllSubForums.First(SubForum => SubForum.Id == subForumId);
       
      
    }
  
    public async Task<SubForum> AddSubForumAsync(SubForum newSubForumItem, int forumId) {
        Forum forumById = await GetForumByIdAsync(forumId);
        if (forumById.AllSubForums.Any()) {
            int largestId = forumById.AllSubForums.Max(subForum => subForum.Id);
            newSubForumItem.Id = largestId + 1;
        }
        else {
            newSubForumItem.Id = 1;
        }

        newSubForumItem.CreatedAt = DateTime.Now;
        fileContext.Forums.First(forum => forum.Id == forumId).AllSubForums.Add(newSubForumItem);
        await fileContext.SaveChangesAsync();
        return newSubForumItem;
    }

    
    
    public async Task IncrementViewOfForumAsync(int forumId) {
        (await GetForumByIdAsync(forumId)).Views++;
        await fileContext.SaveChangesAsync();
    }

    public async Task<Post> AddPostAsync(Post newPostItem, int subForumId)
    {
        SubForum subForumById = await GetSubForumAsync(subForumId);
        if (subForumById.AllPosts.Any()) {
            int largestId = subForumById.AllPosts.Max(subForum => subForum.Id);
            newPostItem.Id = largestId + 1;
        }
        else {
            newPostItem.Id = 1;
        }

        newPostItem.CreatedAt = DateTime.Now;
        fileContext.Forums.First().AllSubForums.First(SubForum => 
            subForumById.Id == subForumId).AllPosts.Add(newPostItem);
        await fileContext.SaveChangesAsync();
        return newPostItem;
    }

   /* public async Task IncrementViewOfSubForumAsync(int subForumId)
    {
        (await GetForumByIdAsync(subForumId)).Views++;
        await fileContext.SaveChangesAsync();
    }
*/
    public Task<Post?> GetPostAsync(int postId)
    {
        throw new NotImplementedException();
    }

    public Task<Comment> AddCommentToPost(int postId, Comment commentToPost)
    {
        throw new NotImplementedException();
    }

    public Task<Comment> EditComment(Comment editedComment)
    {
        throw new NotImplementedException();
    }

 

   public Task<Comment> DeleteComment(int commentId)
   {
       throw new NotImplementedException();
   }
    public async Task IncrementViewOfSubForumAsync(int subForumId) {
        ((await GetSubForumAsync(subForumId))!).Views++;
        await fileContext.SaveChangesAsync();
    }

    public async Task<Post?> GetPostAsync(int forumId, int subForumId, int postId) {
        Post? first = (await GetSubForumAsync(subForumId))?.AllPosts.First(post => post.Id == postId);
        fileContext.Dispose();
        return first;
    }

    public async Task<Comment> AddCommentToPost(int forumId, int subForumId, int postId, Comment commentToPost) {
        Post? post = await GetPostAsync(forumId, subForumId, postId);
        if (post.Comments.Any()) {
            int largestId = post.Comments.Max(comment => comment.Id);
            commentToPost.Id = largestId + 1;
        }
        else {
            commentToPost.Id = 1;
        }

        fileContext.Forums.First(forum => forum.Id == forumId).AllSubForums.First(subForum => subForum.Id == subForumId)
            .AllPosts.First(post => post.Id == postId).Comments.Add(commentToPost);
        await fileContext.SaveChangesAsync();
        return commentToPost;
    }

    public async Task<Comment> EditComment(int forumId, int subForumId, int postId, Comment editedComment) {
        Post? post = await GetPostAsync(forumId, subForumId, postId);
        Comment commentFromDb = fileContext.Forums.First(forum => forum.Id == forumId).AllSubForums
            .First(subForum => subForum.Id == subForumId)
            .AllPosts.First(post => post.Id == postId).Comments.First(comment => comment.Id == editedComment.Id);

        commentFromDb.Body = editedComment.Body;
        commentFromDb.Id = editedComment.Id;
        commentFromDb.Writer = editedComment.Writer;
        commentFromDb.CreatedAt = editedComment.CreatedAt;
     
        await fileContext.SaveChangesAsync();
        return commentFromDb;
    }

    public async Task<Comment> DeleteComment(int forumId, int subForumId, int postId, int commentId) {
        Comment commentFromDb = fileContext.Forums.First(forum => forum.Id == forumId).AllSubForums
            .First(subForum => subForum.Id == subForumId).AllPosts.First(post => post.Id == postId).Comments
            .First(comment1 => comment1.Id == commentId);
        fileContext.Forums.First(forum => forum.Id == forumId).AllSubForums
            .First(subForum => subForum.Id == subForumId).AllPosts.First(post => post.Id == postId).Comments
            .Remove(commentFromDb);
       await fileContext.SaveChangesAsync();
       return commentFromDb;

    }

   
}