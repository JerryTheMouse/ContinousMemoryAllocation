 namespace MemoryManagement.Data
{
    internal class Hole
    {
        public uint id { get; }
        public uint BaseReg { get; }
        
        public uint Size { get; set; }
        private static uint _counter = 0;
        public Hole(uint baseReg, uint size)
        {
            _counter++;
            id = _counter;
            
            this.BaseReg = baseReg;
            this.Size = size;

        }

   
    }
}
