namespace raupjc_obg.Models.ContentViewModels
{
    public class EventViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public string GameName { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Repeat { get; set; }
        public bool HappensOnce { get; set; }

        public string NextEventName { get; set; }
        public string Behaviour { get; set; }
        public string Items { get; set; }
    }
}