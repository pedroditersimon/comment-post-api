﻿using CommentPost.Application.Mappers;
using CommentPost.Application.Services;
using CommentPost.Domain.Entities;

namespace CommentPost.Application.UseCases.Comments.Moderator;

// Editar un comentario: Un moderador edita un comentario.

public class UpdateCommentByModCommand
{
	public int CommentId { get; set; }

	// replaced values
	public int? UserId { get; set; }
	public string? PageId { get; set; }
	public string? Text { get; set; }
	public bool? Visibility { get; set; } = true;
	public int? ReplyId { get; set; }
}

public class UpdateCommentByModHandler
{
	readonly ICommentService _commentService;
	readonly IUnitOfWork _unitOfWork;

	public UpdateCommentByModHandler(ICommentService commentService, IUnitOfWork unitOfWork)
	{
		_commentService = commentService;
		_unitOfWork = unitOfWork;
	}

	public async Task<Comment> ExecuteAsync(UpdateCommentByModCommand command)
	{
		// get original
		Comment? comment = await _commentService.GetById(command.CommentId);
		if (command == null)
			throw new Exception("Comment Not found");

		// replace values
		Comment? replacedComment = comment.ReplaceWith(command, skipNullValues: true);
		if (replacedComment == null)
			throw new Exception("Cannot replace the comment values");

		// update
		Comment? updatedComment = await _commentService.Update(replacedComment);
		if (updatedComment == null)
			throw new Exception("Cannot update the Comment");

		// save
		bool saved = await _unitOfWork.ApplyChangesAsync();
		if (!saved)
			throw new Exception("Cannot save");

		return updatedComment;
	}
}
