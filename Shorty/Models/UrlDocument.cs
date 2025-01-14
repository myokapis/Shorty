using System.ComponentModel.DataAnnotations;

namespace Shorty.Models
{
    // NOTE: using a simple data model for this POC. In a real app, there would likely
    //       be additional requirements around analytics, billing, reporting, etc. The
    //       document structure would need to accommodate those requirements. Depending
    //       upon the database engine selected, the structure could change to better
    //       support data access.
    public class UrlDocument
    {
        public DateTimeOffset? ExpiresAfter { get; set; }
        public required string FullUrl { get; set; }
        public required string Tag { get; set; }
    }
}
