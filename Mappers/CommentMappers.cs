using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Models;

namespace api.Mappers
{
    public static class CommentMappers
    {
        public static CommentDto ToCommentDto(this Comment commentModel){
            return new CommentDto{
                Id = commentModel.Id,
                Title = commentModel.Title,
                Contents = commentModel.Contents,
                CreatedOn = commentModel.CreatedOn,
                StockId = commentModel.StockId
            };
        }
          public static Comment ToCommentFromCreate(this CreateCommentDto commentDto, int stockId){
            return new Comment{
                Title = commentDto.Title,
                Contents = commentDto.Contents,
                StockId = stockId
            };
        }
          public static Comment ToCommentFromUpdate(this UpdateCommentDto commentDto){
            return new Comment{
                Title = commentDto.Title,
                Contents = commentDto.Contents,
            };
        }
    }
}