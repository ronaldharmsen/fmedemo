namespace ProxyServer
{
    internal class FileProcessed
    {
        private string file;
        public string File => file;
        public FileProcessed(string file)
        {
            this.file = file;
        }
    }
}