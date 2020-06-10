namespace Validot.Tests.Functional.Documentation.Models
{
    using System.Collections.Generic;

    public class BookModel
    {
        public string Title { get; set; }

        public IEnumerable<AuthorModel> Authors { get; set; }

        public IEnumerable<LanguageEnum> Languages { get; set; }

        public int YearOfFirstAnnouncement { get; set; }

        public int? YearOfPublication { get; set; }

        public PublisherModel Publisher { get; set; }

        public bool IsSelfPublished { get; set; }
    }
}
