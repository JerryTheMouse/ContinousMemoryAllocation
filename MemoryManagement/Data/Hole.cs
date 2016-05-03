 namespace MemoryManagement.Data
{
    internal class Hole
    {
        public uint BaseReg { get; }
        
        public uint Size { get; set; }
        public Hole(uint baseReg, uint size)
        {
            this.BaseReg = baseReg;
            this.Size = size;

        }

   
    }
}
