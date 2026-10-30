using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

public class PlayerDealerHandler
{
	public bool needsSave;

	private List<int> allocatedIDs = new List<int>();

	public PlayerDealer currentDealer;

	public List<PedTeam> teams = new List<PedTeam>();

	public List<Vector3> HealLocations = new List<Vector3>
	{
		new Vector3(342.7698f, -1397.948f, 32.50927f),
		new Vector3(298.8214f, -584.4205f, 43.26083f),
		new Vector3(355.9612f, -596.3049f, 28.77687f),
		new Vector3(-448.425f, -340.619f, 34.50178f),
		new Vector3(1839.235f, 3673.061f, 34.27673f)
	};

	public Enums.HealPositionTypes HealPositionType;

	public List<PlayerDealer> dealers { get; set; } = new List<PlayerDealer>();

	public PlayerDealerHandler()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		LoadDealers();
	}

	public void OnTick()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (currentDealer != null)
		{
			Vector3 position = ((Entity)currentDealer.Ped).Position;
			if (position.DistanceTo(LSL.playerPos) < 5f)
			{
				World.DrawMarker((MarkerType)0, ((Entity)currentDealer.Ped).Position + new Vector3(0f, 0f, 1.5f), Vector3.Zero, Vector3.Zero, new Vector3(0.2f, 0.2f, 0.2f), Color.Green);
			}
		}
		if (dealers.Count <= 0)
		{
			return;
		}
		foreach (PlayerDealer dealer in dealers)
		{
			if (dealer == null)
			{
				dealers.Remove(dealer);
				break;
			}
			dealer.OnTick();
		}
		if (needsSave)
		{
			SaveDealers();
			needsSave = false;
		}
	}

	public void OnSlowTick()
	{
		if (dealers.Count <= 0)
		{
			return;
		}
		foreach (PlayerDealer dealer in dealers.ToList())
		{
			if (dealer == null)
			{
				dealers.Remove(dealer);
				continue;
			}
			if (dealer.CurrentState != Enums.PdStates.Dead && dealer.CurrentState != Enums.PdStates.FollowPlayer && dealer.CurrentState != Enums.PdStates.FollowingPlayer && LsFunctions.TimeChanged && LsFunctions.HoursPassed > 0)
			{
				WorkerAlerts.Observe(dealer);
				dealer.SimulateSales(LsFunctions.HoursPassed);
				needsSave = true;
			}
			if (dealer.CurrentState == Enums.PdStates.Dead)
			{
				WorkerAlerts.Forget(dealer.ID);
				DeleteDealer(dealer.ID);
				dealers.Remove(dealer);
				break;
			}
			if (dealer != null)
			{
				dealer.OnSlowTick();
				WorkerAlerts.Observe(dealer);
			}
		}
	}

	private void LoadDealers()
	{
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0904: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0979: Unknown result type (might be due to invalid IL or missing references)
		//IL_097e: Unknown result type (might be due to invalid IL or missing references)
		XDocument dealerDoc;
		if (!File.Exists("scripts\\LSLife\\LSLife_Dealers.xml"))
		{
			dealerDoc = new XDocument(new XElement("dealers"));
			dealerDoc.Save("scripts\\LSLife\\LSLife_Dealers.xml");
		}
		else
		{
			dealerDoc = XDocument.Load("scripts\\LSLife\\LSLife_Dealers.xml");
		}
		if (dealerDoc.Descendants().ToList().Count <= 0)
		{
			return;
		}
		List<int> usedIDs = new List<int>();
		foreach (XElement element in dealerDoc.Descendants())
		{
			if (!(element.Name == "dealer"))
			{
				continue;
			}
			foreach (XElement ele in element.Elements())
			{
				bool found = true;
				if (!(ele.Name == "id"))
				{
					continue;
				}
				int.TryParse(ele.Value, out var id);
				if (usedIDs.Count > 0)
				{
					while (found)
					{
						if (usedIDs.FindAll((int d) => d == id).Count == 0)
						{
							found = false;
						}
						else
						{
							id++;
						}
					}
				}
				else
				{
					found = false;
				}
				if (!found)
				{
					usedIDs.Add(id);
					ele.Value = id.ToString();
				}
			}
		}
		dealerDoc.Save("scripts\\LSLife\\LSLife_Dealers.xml");
		foreach (XElement ele2 in dealerDoc.Descendants())
		{
			if (!(ele2.Name == "dealer"))
			{
				continue;
			}
			int id2 = 0;
			PedHash pedHash = (PedHash)1413662315;
			Vector3 pos = Vector3.Zero;
			int weed = 0;
			int crack = 0;
			int cocaine = 0;
			int money = 0;
			int armour = -1;
			List<XElement> tWeapons = new List<XElement>();
			Dictionary<int, Tuple<int, int>> pData = new Dictionary<int, Tuple<int, int>>();
			foreach (XElement e in ele2.Elements())
			{
				if (e.Name == "id")
				{
					int.TryParse(e.Value, out id2);
				}
				if (e.Name == "hash")
				{
					Enum.TryParse<PedHash>(e.Value, out pedHash);
				}
				if (e.Name == "posX")
				{
					pos.X = LsFunctions.StringToFloat(e.Value);
				}
				if (e.Name == "posY")
				{
					pos.Y = LsFunctions.StringToFloat(e.Value);
				}
				if (e.Name == "posZ")
				{
					pos.Z = LsFunctions.StringToFloat(e.Value);
				}
				if (e.Name == "weed")
				{
					int.TryParse(e.Value, out weed);
				}
				if (e.Name == "crack")
				{
					int.TryParse(e.Value, out crack);
				}
				if (e.Name == "cocaine")
				{
					int.TryParse(e.Value, out cocaine);
				}
				if (e.Name == "money")
				{
					int.TryParse(e.Value, out money);
				}
				if (e.Name == "armour")
				{
					int.TryParse(e.Value, out armour);
				}
				if (e.Name == "weapon")
				{
					tWeapons.Add(e);
				}
				if (!(e.Name == "pedData"))
				{
					continue;
				}
				int headD = 0;
				int headT = 0;
				int beardD = 0;
				int beardT = 0;
				int hairD = 0;
				int hairT = 0;
				int torsoD = 0;
				int torsoT = 0;
				int legsD = 0;
				int legsT = 0;
				int handsD = 0;
				int handsT = 0;
				int footD = 0;
				int footT = 0;
				int acces1D = 0;
				int acces1T = 0;
				int acces2D = 0;
				int acces2T = 0;
				int decalD = 0;
				int decalT = 0;
				int torsoAuxD = 0;
				int torsoAuxT = 0;
				foreach (XElement pD in e.Elements())
				{
					if (pD.Name == "headD")
					{
						int.TryParse(pD.Value, out headD);
					}
					if (pD.Name == "headT")
					{
						int.TryParse(pD.Value, out headT);
					}
					if (pD.Name == "beardD")
					{
						int.TryParse(pD.Value, out beardD);
					}
					if (pD.Name == "beardT")
					{
						int.TryParse(pD.Value, out beardT);
					}
					if (pD.Name == "hairD")
					{
						int.TryParse(pD.Value, out hairD);
					}
					if (pD.Name == "hairT")
					{
						int.TryParse(pD.Value, out hairT);
					}
					if (pD.Name == "torsoD")
					{
						int.TryParse(pD.Value, out torsoD);
					}
					if (pD.Name == "torsoT")
					{
						int.TryParse(pD.Value, out torsoT);
					}
					if (pD.Name == "legsD")
					{
						int.TryParse(pD.Value, out legsD);
					}
					if (pD.Name == "legsT")
					{
						int.TryParse(pD.Value, out legsT);
					}
					if (pD.Name == "handsD")
					{
						int.TryParse(pD.Value, out handsD);
					}
					if (pD.Name == "handsT")
					{
						int.TryParse(pD.Value, out handsT);
					}
					if (pD.Name == "footD")
					{
						int.TryParse(pD.Value, out footD);
					}
					if (pD.Name == "footT")
					{
						int.TryParse(pD.Value, out footT);
					}
					if (pD.Name == "acces1D")
					{
						int.TryParse(pD.Value, out acces1D);
					}
					if (pD.Name == "acces1T")
					{
						int.TryParse(pD.Value, out acces1T);
					}
					if (pD.Name == "acces2D")
					{
						int.TryParse(pD.Value, out acces2D);
					}
					if (pD.Name == "acces2T")
					{
						int.TryParse(pD.Value, out acces2T);
					}
					if (pD.Name == "decalD")
					{
						int.TryParse(pD.Value, out decalD);
					}
					if (pD.Name == "decalT")
					{
						int.TryParse(pD.Value, out decalT);
					}
					if (pD.Name == "torsoAuxD")
					{
						int.TryParse(pD.Value, out torsoAuxD);
					}
					if (pD.Name == "torsoAuxT")
					{
						int.TryParse(pD.Value, out torsoAuxT);
					}
				}
				pData.Add(0, new Tuple<int, int>(headD, headT));
				pData.Add(1, new Tuple<int, int>(beardD, beardT));
				pData.Add(2, new Tuple<int, int>(hairD, hairT));
				pData.Add(3, new Tuple<int, int>(torsoD, torsoT));
				pData.Add(4, new Tuple<int, int>(legsD, legsT));
				pData.Add(5, new Tuple<int, int>(handsD, handsT));
				pData.Add(6, new Tuple<int, int>(footD, footT));
				pData.Add(8, new Tuple<int, int>(acces1D, acces1T));
				pData.Add(9, new Tuple<int, int>(acces2D, acces2T));
				pData.Add(10, new Tuple<int, int>(decalD, decalT));
				pData.Add(11, new Tuple<int, int>(torsoAuxD, torsoAuxT));
			}
			LsFunctions.DbugString("Dealer Postion loaded " + pos.X + ", " + pos.Y + ", " + pos.Z);
			if (pos != Vector3.Zero)
			{
				allocatedIDs.Add(id2);
				if (LSL.DEBUG)
				{
					GTA.UI.Notification.PostTicker("Identifiant " + id2 + " attribué, taille du groupe = " + allocatedIDs.Count, false, false);
				}
				if (dealers.FindAll((PlayerDealer d) =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					return d.area == LsFunctions.GetZoneLabel(pos);
				}).Count == 0)
				{
					AddDealer(new PlayerDealer(id2, pedHash, Enums.PdStates.Work, pos, weed, crack, cocaine, money, armour, tWeapons, pData));
				}
				else
				{
					AddDealer(new PlayerDealer(id2, pedHash, Enums.PdStates.FollowPlayer, pos, weed, crack, cocaine, money, armour, tWeapons, pData));
				}
				if (LSL.DEBUG)
				{
					GTA.UI.Notification.PostTicker("Dealer chargé : " + id2, false, false);
				}
			}
		}
	}

	private void SaveDealers()
	{
		XDocument dealerDoc1 = XDocument.Load("scripts\\LSLife\\LSLife_Dealers.xml");
		foreach (PlayerDealer pdealer in dealers)
		{
			SaveDealer(pdealer, dealerDoc1);
		}
		dealerDoc1.Save("scripts\\LSLife\\LSLife_Dealers.xml");
	}

	public Vector3 GetCloseHealPosition(Vector3 _pedPos)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		Vector3 house = Vector3.Zero;
		if (LSL.HouseHandler.Houses.Count > 0)
		{
			List<StashHouse> cHouses = LSL.HouseHandler.Houses.Where((StashHouse stashHouse) =>
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				Vector3 entrancePos2 = stashHouse.entrancePos;
				return entrancePos2.DistanceTo(_pedPos) < 40f;
			}).ToList();
			if (cHouses != null && cHouses.Count > 0)
			{
				StashHouse cHouse = cHouses[0];
				foreach (StashHouse h in cHouses)
				{
					Vector3 entrancePos = cHouse.entrancePos;
					float num = entrancePos.DistanceTo(_pedPos);
					entrancePos = h.entrancePos;
					if (num < entrancePos.DistanceTo(_pedPos))
					{
						cHouse = h;
					}
				}
				house = cHouse.entrancePos;
			}
		}
		Vector3 hospital = Vector3.Zero;
		if (LSL.DealerHandler.HealLocations.Count > 0)
		{
			List<Vector3> cHospitals = LSL.DealerHandler.HealLocations.Where((Vector3 val) =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				return val.DistanceTo(LSL.playerPos) < 50f;
			}).ToList();
			if (cHospitals != null && cHospitals.Count > 0)
			{
				Vector3 cHospital = cHospitals[0];
				foreach (Vector3 item in cHospitals)
				{
					Vector3 h2 = item;
					if (h2.DistanceTo(LSL.playerPos) < cHospital.DistanceTo(LSL.playerPos))
					{
						cHospital = h2;
					}
				}
				hospital = cHospital;
			}
		}
		if (house != Vector3.Zero && hospital != Vector3.Zero)
		{
			if (house.DistanceTo(LSL.playerPos) < hospital.DistanceTo(LSL.playerPos))
			{
				HealPositionType = Enums.HealPositionTypes.House;
				return house;
			}
			HealPositionType = Enums.HealPositionTypes.Hospital;
			return hospital;
		}
		if (house != Vector3.Zero)
		{
			HealPositionType = Enums.HealPositionTypes.House;
			return house;
		}
		if (hospital != Vector3.Zero)
		{
			HealPositionType = Enums.HealPositionTypes.Hospital;
			return hospital;
		}
		HealPositionType = Enums.HealPositionTypes.None;
		return Vector3.Zero;
	}

	public int GetNewID()
	{
		int id = 0;
		if (allocatedIDs.FindAll((int i) => i == id).Count != 0)
		{
			id++;
			for (; allocatedIDs.FindAll((int i) => i == id).Count != 0; id++)
			{
			}
		}
		if (LSL.DEBUG)
		{
			GTA.UI.Notification.PostTicker("Nouvel identifiant : " + id, false, false);
		}
		allocatedIDs.Add(id);
		return id;
	}

	public void SaveDealer(PlayerDealer _dealer, XDocument _doc)
	{
		foreach (XElement element in _doc.Descendants())
		{
			if (!(element.Name == "id") || !(element.Value == _dealer.ID.ToString()))
			{
				continue;
			}
			bool foundArmour = false;
			bool foundPedData = false;
			foreach (XElement ele in element.Parent.Elements())
			{
				if (ele.Name == "posX")
				{
					ele.Value = _dealer.Pos.X.ToString();
				}
				if (ele.Name == "posY")
				{
					ele.Value = _dealer.Pos.Y.ToString();
				}
				if (ele.Name == "posZ")
				{
					ele.Value = _dealer.Pos.Z.ToString();
				}
				if (ele.Name == "weed")
				{
					ele.Value = _dealer.Drugs["Weed"].ToString();
				}
				if (ele.Name == "crack")
				{
					ele.Value = _dealer.Drugs["Crack"].ToString();
				}
				if (ele.Name == "cocaine")
				{
					ele.Value = _dealer.Drugs["Cocaine"].ToString();
				}
				if (ele.Name == "money")
				{
					ele.Value = _dealer.Money.ToString();
				}
				if (ele.Name == "armour")
				{
					foundArmour = true;
					ele.Value = _dealer.dArmour.ToString();
				}
				if (ele.Name == "pedData")
				{
					foundPedData = true;
				}
			}
			if (!foundArmour)
			{
				XElement armourEle = new XElement("armour", _dealer.dArmour);
				element.Parent.Add(armourEle);
			}
			if (!foundPedData && (Entity)(object)_dealer.Ped != (Entity)null)
			{
				XElement pedData = new XElement("pedData");
				_dealer.pData = LsFunctions.GetPedVariationData(_dealer.Ped);
				XElement headD = new XElement("headD", _dealer.pData[0].Item1.ToString());
				XElement headT = new XElement("headT", _dealer.pData[0].Item2.ToString());
				pedData.Add(headD);
				pedData.Add(headT);
				XElement beardD = new XElement("beardD", _dealer.pData[1].Item1.ToString());
				XElement beardT = new XElement("beardT", _dealer.pData[1].Item2.ToString());
				pedData.Add(beardD);
				pedData.Add(beardT);
				XElement hairD = new XElement("hairD", _dealer.pData[2].Item1.ToString());
				XElement hairT = new XElement("hairT", _dealer.pData[2].Item2.ToString());
				pedData.Add(hairD);
				pedData.Add(hairT);
				XElement torsoD = new XElement("torsoD", _dealer.pData[3].Item1.ToString());
				XElement torsoT = new XElement("torsoT", _dealer.pData[3].Item2.ToString());
				pedData.Add(torsoD);
				pedData.Add(torsoT);
				XElement legsD = new XElement("legsD", _dealer.pData[4].Item1.ToString());
				XElement legsT = new XElement("legsT", _dealer.pData[4].Item2.ToString());
				pedData.Add(legsD);
				pedData.Add(legsT);
				XElement handsD = new XElement("handsD", _dealer.pData[5].Item1.ToString());
				XElement handsT = new XElement("handsT", _dealer.pData[5].Item2.ToString());
				pedData.Add(handsD);
				pedData.Add(handsT);
				XElement footD = new XElement("footD", _dealer.pData[6].Item1.ToString());
				XElement footT = new XElement("footT", _dealer.pData[6].Item2.ToString());
				pedData.Add(footD);
				pedData.Add(footT);
				XElement acces1D = new XElement("acces1D", _dealer.pData[8].Item1.ToString());
				XElement acces1T = new XElement("acces1T", _dealer.pData[8].Item2.ToString());
				pedData.Add(acces1D);
				pedData.Add(acces1T);
				XElement acces2D = new XElement("acces2D", _dealer.pData[9].Item1.ToString());
				XElement acces2T = new XElement("acces2T", _dealer.pData[9].Item2.ToString());
				pedData.Add(acces2D);
				pedData.Add(acces2T);
				XElement decalD = new XElement("decalD", _dealer.pData[10].Item1.ToString());
				XElement decalT = new XElement("decalT", _dealer.pData[10].Item2.ToString());
				pedData.Add(decalD);
				pedData.Add(decalT);
				XElement torsoAuxD = new XElement("torsoAuxD", _dealer.pData[11].Item1.ToString());
				XElement torsoAuxT = new XElement("torsoAuxT", _dealer.pData[11].Item2.ToString());
				pedData.Add(torsoAuxD);
				pedData.Add(torsoAuxT);
				element.Parent.Add(pedData);
			}
			break;
		}
	}

	private void DeleteDealer(int _id)
	{
		XDocument dealerDoc = XDocument.Load("scripts\\LSLife\\LSLife_Dealers.xml");
		if (dealerDoc.Descendants().ToList().Count <= 0 || dealers.Count <= 0)
		{
			return;
		}
		foreach (PlayerDealer dealer in dealers.ToList())
		{
			if (dealer.ID != _id)
			{
				continue;
			}
			dealers.Remove(dealer);
			XElement dealerEle = null;
			foreach (XElement ele in dealerDoc.Descendants())
			{
				if (!(ele.Name == "dealer"))
				{
					continue;
				}
				foreach (XElement e in ele.Elements())
				{
					if (e.Value == _id.ToString())
					{
						dealerEle = e.Parent;
						break;
					}
				}
			}
			dealerEle?.Remove();
			break;
		}
		dealerDoc.Save("scripts\\LSLife\\LSLife_Dealers.xml");
	}

	public void AddDealer(PlayerDealer dealer)
	{
		dealers.Add(dealer);
	}

	public bool CreateTeam(PlayerDealer _dealer)
	{
		if (teams.Count > 0)
		{
			if (teams.Where((PedTeam t) => t.Area == LSL.playerArea).Count() == 0)
			{
				if (LsFunctions.CurrentRepLvl(LSL.areas[LSL.areaIndex].Reputation) >= 1.0)
				{
					teams.Add(new PedTeam(_dealer, LSL.playerArea));
					return true;
				}
				LsFunctions.SendTextMsg("Tu dois gagner davantage de réputation dans ce quartier avant d'y envoyer une équipe en patrouille.");
			}
			else
			{
				LsFunctions.SendTextMsg("Une équipe patrouille déjà dans ce quartier.");
			}
		}
		return false;
	}
}
