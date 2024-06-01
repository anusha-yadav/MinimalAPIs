using LibraryManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<LibraryDBContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("LibraryDB")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // To get all books
            app.MapGet("/book", async (LibraryDBContext context) =>
            {
                try
                {
                    var books = await context.Books.ToListAsync();
                    return Results.Ok(books);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"An error occurred: {ex}");

                    // Return a 500 Internal Server Error response
                    return Results.Problem("An error occurred while processing your request. Please try again later.");
                }
            });


            //To get books based on id
            app.MapGet("/books/{id}", async (int id, LibraryDBContext db) =>
            {
                try
                {
                    var book = await db.Books
                        .Include(b => b.Author)
                        .FirstOrDefaultAsync(b => b.BookId == id);

                    if (book != null)
                    {
                        return Results.Ok(book);
                    }
                    else
                    {
                        return Results.NotFound();
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"An error occurred: {ex}");

                    // Return a 500 Internal Server Error response
                    return Results.Problem("An error occurred while processing your request. Please try again later.");
                }
            });

            // To create a new book
            app.MapPost("/books", async (Books book, LibraryDBContext db) =>
            {
                try
                {
                    var validationResults = new List<ValidationResult>();
                    var context = new ValidationContext(book);

                    if (!Validator.TryValidateObject(book, context, validationResults, true))
                    {
                        return Results.BadRequest(validationResults);
                    }

                    var author = await db.Author.FindAsync(book.AuthorId);
                    if (author == null)
                    {
                        return Results.BadRequest("Author not found");
                    }

                    var isbnExists = await db.Books.AnyAsync(b => b.ISBN == book.ISBN);
                    if (isbnExists)
                    {
                        return Results.BadRequest("ISBN must be unique");
                    }

                    db.Books.Add(book);
                    await db.SaveChangesAsync();
                    return Results.Created($"/books/{book.BookId}", book);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"An error occurred: {ex}");

                    // Return a 500 Internal Server Error response
                    return Results.Problem("An error occurred while processing your request. Please try again later.");
                }
            });

            //To Create Author
            app.MapPost("/authors", async (Author author, LibraryDBContext db) =>
            {
                try
                {
                    var validationResults = new List<ValidationResult>();
                    var context = new ValidationContext(author);

                    if (!Validator.TryValidateObject(author, context, validationResults, true))
                    {
                        return Results.BadRequest(validationResults);
                    }

                    db.Author.Add(author);
                    await db.SaveChangesAsync();
                    return Results.Created($"/authors/{author.AuthorId}", author);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"An error occurred: {ex}");

                    // Return a 500 Internal Server Error response
                    return Results.Problem("An error occurred while processing your request. Please try again later.");
                }
            });

            // To update a book based on id
            app.MapPut("/books/{id}", async (int id, Books inputBook, LibraryDBContext db) =>
            {
                try
                {
                    var validationResults = new List<ValidationResult>();
                    var context = new ValidationContext(inputBook);

                    if (!Validator.TryValidateObject(inputBook, context, validationResults, true))
                    {
                        return Results.BadRequest(validationResults);
                    }

                    var book = await db.Books.FindAsync(id);

                    if (book is null) return Results.NotFound();

                    book.Title = inputBook.Title;
                    book.PublicationDate = inputBook.PublicationDate;
                    book.ISBN = inputBook.ISBN;
                    book.AuthorId = inputBook.AuthorId;

                    await db.SaveChangesAsync();

                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"An error occurred: {ex}");

                    // Return a 500 Internal Server Error response
                    return Results.Problem("An error occurred while processing your request. Please try again later.");
                }
            });

            //To delete a book based on id
            app.MapDelete("/books/{id}", async (int id, LibraryDBContext db) =>
            {
                try
                {
                    var book = await db.Books.FindAsync(id);

                    if (book is null) return Results.NotFound();

                    db.Books.Remove(book);
                    await db.SaveChangesAsync();

                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"An error occurred: {ex}");

                    // Return a 500 Internal Server Error response
                    return Results.Problem("An error occurred while processing your request. Please try again later.");
                }
            });

            //To get all authors
            app.MapGet("/authors", async (LibraryDBContext db) =>
            {
                try
                {
                    var authors = await db.Author.ToListAsync();
                    return Results.Ok(authors);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"An error occurred: {ex}");

                    // Return a 500 Internal Server Error response
                    return Results.Problem("An error occurred while processing your request. Please try again later.");
                }
            });

            // To get authors based on id
            app.MapGet("/authors/{id}", async (int id, LibraryDBContext db) =>
            {
                try
                {
                    var author = await db.Author.FindAsync(id);
                    return author is not null ? Results.Ok(author) : Results.NotFound();
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"An error occurred: {ex}");

                    // Return a 500 Internal Server Error response
                    return Results.Problem("An error occurred while processing your request. Please try again later.");
                }
            });

            // To update author record based on id
            app.MapPut("/authors/{id}", async (int id, Author inputAuthor, LibraryDBContext db) =>
            {
                try
                {
                    var validationResults = new List<ValidationResult>();
                    var context = new ValidationContext(inputAuthor);

                    if (!Validator.TryValidateObject(inputAuthor, context, validationResults, true))
                    {
                        return Results.BadRequest(validationResults);
                    }

                    var author = await db.Author.FindAsync(id);

                    if (author is null) return Results.NotFound();

                    author.AuthorName = inputAuthor.AuthorName;

                    await db.SaveChangesAsync();

                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"An error occurred: {ex}");

                    // Return a 500 Internal Server Error response
                    return Results.Problem("An error occurred while processing your request. Please try again later.");
                }
            });

            // To delete author based on id
            app.MapDelete("/authors/{id}", async (int id, LibraryDBContext db) =>
            {
                try
                {
                    var author = await db.Author.FindAsync(id);

                    if (author is null) return Results.NotFound();

                    db.Author.Remove(author);
                    await db.SaveChangesAsync();

                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"An error occurred: {ex}");

                    // Return a 500 Internal Server Error response
                    return Results.Problem("An error occurred while processing your request. Please try again later.");
                }
            });

            app.Run();
        }
    }
}