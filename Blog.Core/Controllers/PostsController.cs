﻿using System.Threading.Tasks;
using Blog.Core.Models.Posts;
using Blog.Core.Services.Foundations.Posts;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace Blog.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : RESTFulController
    {
        private readonly IPostService postService;

        public PostsController(IPostService postService) =>
            this.postService = postService;

        [HttpPost]
        public async ValueTask<ActionResult<Post>> PostPostAsync(Post post)
        {
            Post addedPost =
                await this.postService.AddPostAsync(post);

            return Created(addedPost);
        }
    }
}
