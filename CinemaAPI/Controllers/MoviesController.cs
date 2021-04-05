using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private CinemaDbContext _dbContext;

        public MoviesController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region GET
        [Authorize]
        [HttpGet]
        public IActionResult GetAllMovies(string sort)
        {
            switch(sort)
            {
                case "desc":
                    return Ok(_dbContext.Movies.OrderByDescending(m => m.Rating));
                case "asc":
                    return Ok(_dbContext.Movies.OrderBy(m => m.Rating));
                default:
                    return Ok(_dbContext.Movies);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetMovieDetails(int id)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound("No record found");
            }
            else
            {
                return Ok(movie);
            }
        }
        #endregion

        #region POST
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromForm] Movie movie)
        {
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");
            if (movie.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movie.Image.CopyTo(fileStream);
            }
            movie.ImageUrl = filePath.Remove(0, 7);
            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }
        #endregion

        #region PUT
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] Movie newMovie)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound("No record found");
            }
            else
            {
                var guid = Guid.NewGuid();
                var filePath = Path.Combine("wwwroot", guid + ".jpg");
                if (newMovie.Image != null)
                {
                    var fileStream = new FileStream(filePath, FileMode.Create);
                    newMovie.Image.CopyTo(fileStream);
                    movie.ImageUrl = filePath.Remove(0, 7);
                }
                movie.Name = newMovie.Name;
                movie.Language = newMovie.Language;
                movie.Duration = newMovie.Duration;
                movie.PlayingDate = newMovie.PlayingDate;
                movie.PlayingTime = newMovie.PlayingTime;
                movie.TicketPrice = newMovie.TicketPrice;
                movie.Rating = newMovie.Rating;
                movie.Genre = newMovie.Genre;
                movie.TrailorUrl = newMovie.TrailorUrl;
                _dbContext.SaveChanges();
                return Ok("Record updated succesfully");
            }
        }
        #endregion

        #region DELETE
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound("No record found");
            }
            else
            {
                _dbContext.Movies.Remove(movie);
                _dbContext.SaveChanges();
                return Ok("Record deleted");
            }
        }
        #endregion
    }
}
