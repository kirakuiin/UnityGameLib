using GameLib.Network.NGO;
using NUnit.Framework;

namespace UnitTest.Editor
{
    public struct PlayerTestData : ISessionPlayerData
    {
        public bool IsConnected {set; get;}

        public ulong ClientID {set; get;}

        public string Name { set; get; }
        
        public bool IsReinitialize { set; get; }

        const string Default = "test";

        public static PlayerTestData CreateInstance(string name, ulong clientID)
        {
            return new PlayerTestData()
            {
                IsConnected = true,
                IsReinitialize = false,
                ClientID = clientID,
                Name = name,
            };
        }
        public void Reinitialize()
        {
            IsReinitialize = true;
        }
    }
    
    [TestFixture]
    public class SessionManagerUnitTest
    {
        private SessionManager<PlayerTestData> _manager;
        
        private readonly (string playerName, ulong clientID) _playerInfo1 = ("nico", 0);
        private readonly (string playerName, ulong clientID) _playerInfo2 = ("dracula", 1);
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _manager = new();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _manager.ClearAllData();
        }

        [SetUp]
        public void Setup()
        {
            _manager.StartSession();
            _manager.SetupPlayerData(_playerInfo1.clientID, _playerInfo1.playerName,
                PlayerTestData.CreateInstance(_playerInfo1.playerName, _playerInfo1.clientID));
            _manager.SetupPlayerData(_playerInfo2.clientID, _playerInfo2.playerName,
                PlayerTestData.CreateInstance(_playerInfo2.playerName, _playerInfo2.clientID));
        }

        [TearDown]
        public void TearDown()
        {
            _manager.ClearAllData();
        }

        [Test]
        public void TestDuplicate()
        {
            Assert.IsTrue(_manager.IsDuplicateConnection(_playerInfo1.playerName));
        }

        [Test]
        public void TestGetPlayerID()
        {
            Assert.AreNotEqual(_playerInfo1.playerName, _manager.GetPlayerID(_playerInfo2.clientID));
        }

        [Test]
        public void TestGetPlayerData()
        {
            var playerData = _manager.GetPlayerData(_playerInfo2.clientID);
            
            Assert.IsNotNull(playerData);
            Assert.AreEqual(_playerInfo2.playerName, playerData.Value.Name);
        }

        [Test]
        public void TestUpdatePlayerData()
        {
            var newData = PlayerTestData.CreateInstance("nico", 1);
            
            _manager.UpdatePlayerData(1, newData);
            var result = _manager.GetPlayerData(1);
            
            Assert.IsNotNull(result);
            Assert.AreEqual("nico", newData.Name);
        }

        [Test]
        public void TestDisconnectWithStop()
        {
            _manager.StopSession();
            _manager.DisconnectClient(_playerInfo1.clientID);

            var playerID = _manager.GetPlayerID(_playerInfo1.clientID);
            var playerData = _manager.GetPlayerData(_playerInfo1.clientID);
            Assert.IsNull(playerID);
            Assert.IsNull(playerData); 
        }
        
        [Test]
        public void TestDisconnectWithStart()
        {
            _manager.DisconnectClient(_playerInfo1.clientID);

            var playerID = _manager.GetPlayerID(_playerInfo1.clientID);
            var playerData = _manager.GetPlayerData(_playerInfo1.clientID);
            
            Assert.IsNotNull(playerID);
            Assert.IsNotNull(playerData); 
            Assert.IsFalse(playerData.Value.IsConnected);
        }

        [Test]
        public void TestStopSession()
        {
            _manager.StopSession();

            var playerData = _manager.GetPlayerData(_playerInfo1.clientID);
            
            Assert.IsNotNull(playerData);
            Assert.IsTrue(playerData.Value.IsConnected);
        }

        [Test]
        public void TestReconnect()
        {
            ulong newClientID = 2;
            
            _manager.DisconnectClient(_playerInfo2.clientID);
            _manager.SetupPlayerData(newClientID, _playerInfo2.playerName,
                PlayerTestData.CreateInstance(_playerInfo2.playerName, newClientID));

            var playerData = _manager.GetPlayerData(newClientID);
            
            Assert.IsNotNull(playerData);
            Assert.IsTrue(playerData.Value.IsConnected);
            Assert.AreEqual(newClientID, playerData.Value.ClientID);
        }
    }
}