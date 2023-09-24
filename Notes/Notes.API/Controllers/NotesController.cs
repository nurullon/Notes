using Notes.API.Data;
using Microsoft.AspNetCore.Mvc;
using Notes.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Notes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly NotesDbContext notesDbContext;
    public NotesController(NotesDbContext notesDbContext)
    {
        this.notesDbContext = notesDbContext;
    }
    /// <summary>
    /// Add note in the database
    /// </summary>
    /// <param name="note"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddNoteAsync(Note note)
    {
        await this.notesDbContext.Notes.AddAsync(note);
        await this.notesDbContext.SaveChangesAsync();

        return Ok(note);
    }

    /// <summary>
    /// Update note in the database
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedNote"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> UpdateNoteAsync([FromRoute] long id, [FromBody] Note updatedNote)
    {
        var existNote = await notesDbContext.Notes.FirstOrDefaultAsync(note => note.Id.Equals(id));
        if (existNote is null)
            return NotFound();

        existNote.Title = updatedNote.Title;
        existNote.IsVisible = updatedNote.IsVisible;
        existNote.Description = updatedNote.Description;

        this.notesDbContext.Notes.Update(existNote).State = EntityState.Modified;
        await this.notesDbContext.SaveChangesAsync();

        return Ok(existNote);
    }

    /// <summary>
    /// Delete note from the database
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteNoteAsync([FromRoute] long id)
    {
        var existNote = await notesDbContext.Notes.FirstOrDefaultAsync(note => note.Id.Equals(id));
        if (existNote is null)
            return NotFound();

        this.notesDbContext.Notes.Remove(existNote);
        await this.notesDbContext.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Get note by Id from the database
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id:long}")]
    [ActionName("GetNoteById")]
    public async Task<IActionResult> GetNoteByIdAsync([FromRoute] long id)
    {
        var note = await this.notesDbContext.Notes
                  .FirstOrDefaultAsync(note => note.Id.Equals(id));
        if (note is null)
            return NotFound();

        return Ok(note);
    }

    /// <summary>
    /// Get the notes from the database
    /// </summary>
    /// <returns></returns>
    [HttpGet    ]
    public async Task<IActionResult> GetAllNotesAsync()
    {
        return Ok(await this.notesDbContext.Notes.ToListAsync());
    }
}
