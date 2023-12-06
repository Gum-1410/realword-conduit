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
            var newBlog = await _mediator.Send(request, cancellationToken);
            return Ok(newBlog);
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
            var newBlog = await _mediator.Send(request, cancellationToken);
            return Ok(newBlog);
        }

        [Authorize]
        [HttpDelete("{title}")]
        public async Task<IActionResult> DeleteCurrentBlog([FromRoute] DeleteCurrentBlogCommand request, CancellationToken cancellationToken)
        {
            var newBlog = await _mediator.Send(request, cancellationToken);
            return Ok(newBlog);
        }

        [Authorize]
        [HttpPost("{title}/like")]
        public async Task<IActionResult> LikeBlog([FromRoute] LikeBlogCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{title}/like")]
        public async Task<IActionResult> UnlikeBlog([FromRoute] UnlikeBlogCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
