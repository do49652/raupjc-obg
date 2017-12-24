namespace raupjc_obg.Game.Components
{
    public abstract class Item : HasBehaviour
    {
        public Item()
        {
            SetBehaviour();
        }

        public string Category { get; set; }
    }
}