using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        private readonly UserManager<AppUser> _userManager;

        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo, UserManager<AppUser> userManager)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentRepo.GetAllAsync();
            var commentsDto = comments.Select(s => s.ToCommentDto());
            return Ok(commentsDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId:int}")]
        [Authorize]
        public async Task<IActionResult> Create([FromRoute] int stockId, [FromBody] CreateCommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stockExists = await _stockRepo.StockExists(stockId);

            if (!stockExists)
            {
                return BadRequest("Stock does not exist");
            }

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var comment = commentDto.ToCommentFromCreate(stockId);
            comment.AppUserId = appUser.Id;
            
            await _commentRepo.CreateAsync(comment);

            return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment.ToCommentDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _commentRepo.UpdateAsync(id, commentDto.ToCommentFromUpdate());

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var comment = await _commentRepo.DeleteAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }

    }
}