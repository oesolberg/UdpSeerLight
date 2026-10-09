namespace HSPI_UdpSeerLight
{
    public class FanData
    {
        public string Name { get; set; }

        public decimal Temperature { get; set; }

        public int CommonCurrent { get; set; }

        public int TempRising { get; set; }
        public int SmallMotor { get; set; }
        public int L1Motor { get; set; }
        public int L2Motor { get; set; }
        public int MotorPower { get; set; }

    //"name":"GreenHouseFanData","Temperature":18.125,"CommonCurrent":0,"SmallMotor":0,"L1Motor":0,"L2Motor":0,"TempRising":0 
    }
}