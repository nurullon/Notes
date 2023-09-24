using Notes.API.Data;
using Microsoft.AspNetCore.Mvc;
using Notes.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Notes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : Controller
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
        note.Id = Guid.NewGuid();
        await this.notesDbContext.Notes.AddAsync(note);
        await this.notesDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetNoteByIdAsync), new {id = note.Id}, note);
    }

    /// <summary>
    /// Update note in the database
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedNote"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> UpdateNoteAsync([FromRoute] Guid id, [FromBody] Note updatedNote)
    {
        var existNote = await notesDbContext.Notes.FirstOrDefaultAsync(note => note.Id.Equals(id));
        if (existNote is null)
            return NotFound();

        existNote.Title = updatedNote.Title;
        existNote.IsVisible = updatedNote.IsVisible;
        existNote.Description = updatedNote.Description;

        this.notesDbContext.Notes.Update(existNote);
        await this.notesDbContext.SaveChangesAsync();

        return Ok(existNote);
    }

    /// <summary>
    /// Delete note from the database
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id:Guid}")]
    public async Task<IActionResult> DeleteNoteAsync([FromRoute] Guid id)
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
    [Route("{id:Guid}")]
    [ActionName("GetNoteById")]
    public async Task<IActionResult> GetNoteByIdAsync([FromRoute] Guid id)
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
    [HttpGet("get")]
    public async Task<IActionResult> GetAllNotesAsync()
        => Ok(await this.notesDbContext.Notes.ToListAsync());
}
