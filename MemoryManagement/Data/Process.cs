 namespace MemoryManagement.Data
{
    internal class Process
    {
        public uint id { get; }        
        public uint Size { get; }
        private static uint _counter = 0;
        public Process( uint size)
        {
            _counter++;
            id = _counter;
            
            this.Size = size;

        }

   
    }
}
