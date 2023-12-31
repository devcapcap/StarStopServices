namespace StartStopServices.Models
{
    public class Service
    {
        public Service(){}

		public Service(string index,string name, string status)
		{
			this.Name = name;
			this.Status = status;
            this.IsSelected = false;
		}

        public string Index { get; set; }
        public string Name { get; set; }
		public string Status { get;set;}
        public string StatusColor =>  this.Status.Equals("running", System.StringComparison.InvariantCultureIgnoreCase) ? "Green" : "Red";
        public bool IsSelected { get;  set; }
    }
}
