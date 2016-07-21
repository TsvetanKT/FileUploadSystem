namespace FileUploadSystem.Web.ViewModels.Upload
{
    using PagedList;

    public class UploadViewModel
    {
        public UploadViewModel(IPagedList<FileViewModel> searchResults, string searchLocation, int? currentDirectoryId, string sortBy)
        {
            this.SearchResults = searchResults;
            this.SearchLocation = searchLocation;
            this.CurrentDirectoryId = currentDirectoryId;
            this.SortBy = sortBy;
        }

        public IPagedList<FileViewModel> SearchResults { get; set; }

        public string SearchLocation { get; set; }

        public int? CurrentDirectoryId { get; set; }

        public string SortBy { get; set; }
    }
}