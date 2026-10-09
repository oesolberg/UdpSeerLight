using HSPI_UdpSeerLight;
using NUnit.Framework;
using Should;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace UdpSeer.UnitTesting
{

    [TestFixture]
    public class JsonSplittingTests
    {
        private JsonSplitter _splitter;

        [SetUp]
        public void Setup()
        {
            _splitter = new JsonSplitter();
        }

        [Test]
        public void TestSplitting_Null_ShouldReturnEmptyDictionary()
        {
            var result = _splitter.SplitJsonString(null);

            result.ShouldBeNull();
        }

        [Test]
        public void TestSplitting_EmptyString_ShouldReturnEmptyDictionary()
        {
            var result = _splitter.SplitJsonString(string.Empty);
            
            result.ShouldBeNull();
        }

        [Test]
        public void TestSplitting_EmptyString2_ShouldReturnEmptyDictionary()
        {
            var result = _splitter.SplitJsonString("");

            result.ShouldBeNull();
        }

        [Test]
        public void TestSplitting_StringNotProperlyFilled_ShouldReturnEmptyDictionary()
        {
            var result = _splitter.SplitJsonString("{\"name\":}");

            result.ShouldBeNull();
        }

        [Test]
        public void TestSplitting_StringProperlyFilled_ShouldReturnEmptyDictionary()
        {
            var result = _splitter.SplitJsonString("{\"name\":\"GreenHouseFanData\",\"Temperature\":18.0625}");

            result.Name.ShouldEqual("GreenHouseFanData");
            result.Temperature.ShouldEqual(18.0625m);
        }

        [Test]
        public void TestSplitting_StringProperlyFilledWithCompleteDataSet_ShouldReturnEmptyDictionary()
        {
            var result = _splitter.SplitJsonString("{ \"name\":\"GreenHouseFanData\",\"Temperature\":18.1875,\"CommonCurrent\":1,\"SmallMotor\":1,\"L1Motor\":0,\"L2Motor\":0,\"TempRising\":0,\"MotorPower\":101}");

            result.Name.ShouldEqual("GreenHouseFanData");
            result.Temperature.ShouldEqual(18.1875m);
            result.CommonCurrent.ShouldEqual(1);
            result.SmallMotor.ShouldEqual(1);
            result.L1Motor.ShouldEqual(0);
            result.L2Motor.ShouldEqual(0);
            result.MotorPower.ShouldEqual(101);
        }

    }
}

