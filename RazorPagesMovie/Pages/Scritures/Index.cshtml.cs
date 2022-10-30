using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Scritures
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IList<Scripture> Scripture { get;set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        public SelectList? Books { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? ScriptureBook { get; set; }

        public string? SortBooks { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SortBook { get; set; }

        public async Task OnGetAsync()
        {
            // Use LINQ to get list of genres.
            IQueryable<string> bookQuery = from m in _context.Scripture
                                            orderby m.Book
                                            select m.Book;

            var scriptures = from m in _context.Scripture
                             select m;

            if (!string.IsNullOrEmpty(SearchString))
            {
                scriptures = scriptures.Where(s => s.Note.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(ScriptureBook))
            {
                scriptures = scriptures.Where(x => x.Book == ScriptureBook);
            }

            Scripture = await scriptures.ToListAsync();
            Books = new SelectList(await bookQuery.Distinct().ToListAsync());


            //Trying to sort
            /////////////////
            ViewData["SortBooks"] = String.IsNullOrEmpty(SortBooks) ? "name_desc" : "";
            ViewData["DateSortParm"] = SortBooks == "Date" ? "date_desc" : "Date";
            var sortScriptures = from s in _context.Scripture
                                 select s;
            switch (SortBooks)
            {
                case "name_desc":
                    scriptures = scriptures.OrderByDescending(s => s.Book);
                    break;
                case "Date":
                    scriptures = scriptures.OrderBy(s => s.ReleaseDate);
                    break;
                case "date_desc":
                    scriptures = scriptures.OrderByDescending(s => s.ReleaseDate);
                    break;
            }
            Scripture = await scriptures.ToListAsync();
        }
            
    }
}
