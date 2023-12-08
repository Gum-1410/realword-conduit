using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorldConduit.Application.Articles.Queries;
using RealWorldConduit.Application.Blogs.Commands;
using RealWorldConduit.Application.Blogs.Queries;

namespace Conduit.API.Controllers
{
    [Route("api/blogs")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BlogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetGlobalBlogs([FromQuery] GetPagingGlobalBlogsQuery request, CancellationToken cancellationToken)
        {
            var globalBlogs = await _mediator.Send(request, cancellationToken);
            return Ok(globalBlogs);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateNewBlog([FromBody] CreateBlogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(request, cancellationToken);
                var newBlog = await _mediator.Send(new GetABlogQuery(request.Title), cancellationToken);
                return Ok(newBlog);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize]
        [HttpGet("{title}")]
        public async Task<IActionResult> GetABlog([FromRoute] GetABlogQuery request, CancellationToken cancellationToken)
        {
            var blog = await _mediator.Send(request, cancellationToken);
            return Ok(blog);
        }

        [Authorize]
        [HttpPut("{title}")]
        public async Task<IActionResult> UpdateCurrentBlog([FromBody] UpdateCurrentBlogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(request, cancellationToken);
                var updatedBlog = await _mediator.Send(new GetABlogQuery(request.Title), cancellationToken);
                return Ok(updatedBlog);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize]
        [HttpDelete("{title}")]
        public async Task<IActionResult> DeleteCurrentBlog([FromRoute] DeleteCurrentBlogCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("{title}/like")]
        public async Task<IActionResult> LikeBlogUpsert([FromRoute] LikeBlogUpsertCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(request, cancellationToken);
                var updatedBlog = await _mediator.Send(new GetABlogQuery(request.Title), cancellationToken);
                return Ok(updatedBlog);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
