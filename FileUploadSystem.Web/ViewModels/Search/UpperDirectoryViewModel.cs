namespace FileUploadSystem.Web.ViewModels.Search
{
    public class UpperDirectoryViewModel
    {
        public UpperDirectoryViewModel(object id, string locationName)
        {
            this.Id = id;
            this.LocationName = locationName;
        }

        public object Id { get; set; }
        
        public string LocationName { get; set; }
    }
}