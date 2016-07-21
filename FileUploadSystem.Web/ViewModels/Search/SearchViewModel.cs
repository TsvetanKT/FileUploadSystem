namespace FileUploadSystem.Web.ViewModels.Search
{
    using PagedList;

    public class SearchViewModel
    {
        public SearchViewModel(IPagedList<FileViewModel> searchResults, string searchLocation, bool inUserPage, string sortBy)
        {
            this.SearchResults = searchResults;
            this.SearchLocation = searchLocation;
            this.InUserPage = inUserPage;
            this.SortBy = sortBy;
        }

        public IPagedList<FileViewModel> SearchResults { get; set; }

        public string SearchLocation { get; set; }

        public bool InUserPage { get; set; }

        public string SortBy { get; set; }
    }
}