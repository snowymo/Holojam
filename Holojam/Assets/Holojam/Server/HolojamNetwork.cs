﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Xml;
using System.IO;
using ProtoBuf;
using update_protocol_v3;
using System.Threading;

namespace Holojam.Network {
     public class HolojamNetwork : Singleton<HolojamNetwork> {

          [System.NonSerialized]
          public int sentPacketsPerSecond;
          [System.NonSerialized]
          public int receivedPacketsPerSecond;         

          //Constant and Read-only
          public const int BLACK_BOX_CLIENT_PORT = 1611; //Port for receiving information
          public const int BLACK_BOX_SERVER_PORT = 1615; //Port for sending information

          private HolojamSendThread sendThread;
          private HolojamRecieveThread receiveThread;

          void Start() {
               sendThread = new HolojamSendThread(BLACK_BOX_SERVER_PORT);
               receiveThread = new HolojamRecieveThread(BLACK_BOX_CLIENT_PORT);

               sendThread.Start();
               receiveThread.Start();

			StartCoroutine (DisplayPacketsPerSecond());
          }

          void Update() {
               List<HolojamView> viewsToSend = new List<HolojamView>();

               foreach (HolojamView view in HolojamView.instances) {
                    if (view.isMine) {
                         viewsToSend.Add(view);
                    } else {
					HolojamObject o;
					if (receiveThread.GetObject(view.label, out o)) {
						view.rawPosition = o.position;
						view.rawRotation = o.rotation;
						view.bits = o.bits;
						view.blob = o.blob;
						view.IsTracked = true;
					} else {
						view.IsTracked = false;
					}
				}
               }

               sendThread.UpdateManagedObjects(viewsToSend.ToArray());
          }

		private IEnumerator DisplayPacketsPerSecond() {
			while (receiveThread.IsRunning) {
				yield return new WaitForSeconds(1f);
				//Debug.LogWarning(string.Format("Packets per second: {0} Most recent packet frame: {1}", packetCount, currentPacket.frame));
				sentPacketsPerSecond = sendThread.PacketCount;
				sendThread.PacketCount = 0;
				receivedPacketsPerSecond = receiveThread.PacketCount;
				receiveThread.PacketCount = 0;
				Debug.LogWarning ("HolojamNetwork: Sent Packets - " + sentPacketsPerSecond + " Received Packets - " + receivedPacketsPerSecond);
			}
		}

          public bool IsTracked(string label) {
               HolojamObject o;
               return receiveThread.GetObject(label, out o);
          }

          public bool GetPosition(string label, out Vector3 position) {
               position = HolojamObject.DEFAULT_POSITION;
               HolojamObject o;
               if (receiveThread.GetObject(label, out o)) {
                    position = o.position;
                    return true;
               } else {
                    return false;
               }
          }

          public bool GetRotation(string label, out Quaternion rotation) {
               rotation = HolojamObject.DEFAULT_ROTATION;
               HolojamObject o;
               if (receiveThread.GetObject(label, out o)) {
                    rotation = o.rotation;
                    return true;
               } else {
                    return false;
               }
          }

          public bool GetBits(string label, out int bits) {
               bits = 0;
               HolojamObject o;
               if (receiveThread.GetObject(label, out o)) {
                    bits = o.bits;
                    return true;
               } else {
                    return false;
               }
          }

          public bool GetBlob(string label, out string blob) {
               blob = "";
               HolojamObject o;
               if (receiveThread.GetObject(label, out o)) {
                    blob = o.blob;
                    return true;
               } else {
                    return false;
               }
          }


     }


     internal abstract class HolojamThread {

		protected Thread thread;

          protected int port;
          protected Dictionary<string, HolojamObject> managedObjects = new Dictionary<string, HolojamObject>();
          protected UnityEngine.Object lockObject = new UnityEngine.Object();
          protected int packetCount = 0;
          protected bool isRunning = false;

          protected abstract ThreadStart ThreadStart {
               get;
          }

		public int PacketCount {
               get { return packetCount; }
			set { packetCount = value; }
          }

		public bool IsRunning {
               get { return isRunning; }
          }

          protected HolojamThread(int port) {
               this.port = port;
               thread = new Thread(ThreadStart);
          }

          public void Start() {
               if (this.isRunning) {
                    Debug.LogWarning("Thread already started!");
                    return;
               }

               isRunning = true;
               thread.Start();
          }

          public void Stop() {
               if (!this.isRunning) {
                    Debug.LogWarning("Thread already stopped!");
                    return;
               }
               isRunning = false;
          }

          public bool GetObject(string key, out HolojamObject holoObject) {
               holoObject = null;
               lock (lockObject) {
                    if (managedObjects.ContainsKey(key)) {
                         holoObject = managedObjects[key];
                         return true;
                    } else {
                         return false;
                    }
               }
          }
     }

     internal class HolojamRecieveThread : HolojamThread {

          private PacketBuffer previousPacket = new PacketBuffer(PacketBuffer.PACKET_SIZE);
          private PacketBuffer currentPacket = new PacketBuffer(PacketBuffer.PACKET_SIZE);
          private PacketBuffer tempPacket = new PacketBuffer(PacketBuffer.PACKET_SIZE);
          private update_protocol_v3.Update update;

          protected override ThreadStart ThreadStart {
               get {
                    return Receive;
               }
          }

          public HolojamRecieveThread(int port) : base(port) { }

          public void Receive() {
               Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
               socket.Bind(new IPEndPoint(IPAddress.Any, port));
               socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse("224.1.1.1")));

               int nBytesReceived = 0;
               while (isRunning) {
                    nBytesReceived = socket.Receive(currentPacket.bytes);
                    currentPacket.stream.Position = 0;

                    update = Serializer.Deserialize<update_protocol_v3.Update>(new MemoryStream(currentPacket.bytes, 0, nBytesReceived));

                    currentPacket.frame = update.mod_version;
                    if (currentPacket.frame > previousPacket.frame) {
                         packetCount++;

                         previousPacket.stream.Position = 0;
                         currentPacket.stream.Position = 0;
                         tempPacket.copyFrom(previousPacket);
                         previousPacket.copyFrom(currentPacket);
                         currentPacket.copyFrom(tempPacket);
                         for (int j = 0; j < update.live_objects.Count; j++) {
                              LiveObject or = update.live_objects[j];
                              string label = or.label;

                              HolojamObject ho;
                              lock (lockObject) {
                                   //Reform managedObjects every frame.
                                   //Inefficient for now, but will allow us to determine
                                   //if an object is registered or not.
                                   managedObjects.Clear();

                                   ho = new HolojamObject(label);
                                   managedObjects[label] = ho;

                                   if (update.lhs_frame) {
                                        ho.position = new Vector3(-(float)or.x, (float)or.y, (float)or.z);
                                        ho.rotation = new Quaternion(-(float)or.qx, (float)or.qy, (float)or.qz, -(float)or.qw);
                                   } else {
                                        ho.position = new Vector3((float)or.x, (float)or.y, (float)or.z);
                                        ho.rotation = new Quaternion((float)or.qx, (float)or.qy, (float)or.qz, (float)or.qw);
                                   }
                                   ho.bits = or.button_bits;

                                   //Get blob if it's there. Inefficient
                                   if (or.extra_data.Count > 0) {
                                        ExtraData data = or.extra_data[0];
                                        ho.blob = data.string_val;
                                   }
                              }
                         }
                    }

                    if (!isRunning) {
                         socket.Close();
                         break;
                    }
               }
          }
     }

     internal class HolojamSendThread : HolojamThread {

          private int lastLoadedFrame;
          private byte[] packetBytes;
          private IPAddress ip = IPAddress.Parse("192.168.1.44");
          private update_protocol_v3.Update update;

          protected override ThreadStart ThreadStart {
               get {
                    return Send;
               }
          }

          public HolojamSendThread(int port) : base(port) { }

          public void Send() {
               Debug.Log("Attempting to open send thread with ip/port: " + ip.ToString() + " " + port);
               Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
               IPEndPoint ipEndPoint = new IPEndPoint(ip, 0);
               IPEndPoint send_ipEndPoint = new IPEndPoint(ip, port);

               try {
                    socket.Bind(ipEndPoint);
               } catch (SocketException e) {
                    Debug.Log("Error binding socket: " + ip.ToString() + " " + port + " " + e.ToString());
                    isRunning = false;
               }

			while (isRunning) {
				System.Threading.Thread.Sleep (100);
				if (managedObjects.Values.Count == 0)
					continue;

                    lock(lockObject) {

                         update = new update_protocol_v3.Update();
                         update.label = "neuron";
                         update.mod_version = lastLoadedFrame;
                         update.lhs_frame = false;
                         lastLoadedFrame++;
                         
                         foreach (KeyValuePair<string, HolojamObject> entry in managedObjects) {
                              LiveObject o = entry.Value.ToLiveObject();
                              update.live_objects.Add(o);
                         }

                         using (MemoryStream stream = new MemoryStream()) {
						packetCount++;
                              Serializer.Serialize<Update>(stream, update);
                              packetBytes = stream.GetBuffer();
                              socket.SendTo(packetBytes, send_ipEndPoint);
                         }
                    }

                    if (!isRunning) {
                         socket.Close();
                         break;
                    }
               }
          }

          public void UpdateManagedObjects(HolojamView[] views) {
               managedObjects.Clear();

               lock(lockObject) {
                    foreach (HolojamView view in views) {
                         HolojamObject o = HolojamObject.FromView(view);
                         managedObjects[o.label] = o;
                    }
               }
          }
     }

     internal class HolojamObject {
          public static readonly Vector3 DEFAULT_POSITION = Vector3.zero;
          public static readonly Quaternion DEFAULT_ROTATION = Quaternion.identity;

          public string label;
          public Vector3 position = DEFAULT_POSITION;
          public Quaternion rotation = DEFAULT_ROTATION;
          public int bits = 0;
          public string blob = "";

          public HolojamObject(string label) {
               this.label = label;
          }

          public update_protocol_v3.LiveObject ToLiveObject() {
               update_protocol_v3.LiveObject o = new update_protocol_v3.LiveObject();
               o.label = this.label;

               o.x = position.x;
               o.y = position.y;
               o.z = position.z;

               o.qx = rotation.x;
               o.qy = rotation.y;
               o.qz = rotation.z;
               o.qw = rotation.w;

               o.button_bits = bits;

               if (!string.IsNullOrEmpty(blob)) {
                    ExtraData data = new ExtraData();
                    data.label = "blob";
                    data.string_val = blob;

                    o.extra_data.Add(data);
               }

               return o;
          }

          public static HolojamObject FromView(HolojamView view) {
               HolojamObject o = new HolojamObject(view.label);

               o.position = view.rawPosition;
               o.rotation = view.rawRotation;
               o.bits = view.bits;
               o.blob = view.blob;

               return o;
          }
     }

     internal class PacketBuffer {
          public const int PACKET_SIZE = 65507; // ~65KB buffer sizes

          public byte[] bytes;
          public MemoryStream stream;
          public long frame;

          public PacketBuffer(int packetSize) {
               bytes = new byte[packetSize];
               stream = new MemoryStream(bytes);
               frame = 0;
          }

          public void copyFrom(PacketBuffer other) {
               this.bytes = other.bytes;
               this.stream = other.stream;
               this.frame = other.frame;
          }
     }

     public class Motive {
          public enum LiveObjectTag {
               HEADSET1, HEADSET2, HEADSET3, HEADSET4, WAND1, WAND2, WAND3, WAND4, BOX1, BOX2, SPHERE1,
               LEFTHAND1, RIGHTHAND1, LEFTFOOT1, RIGHTFOOT1, LEFTHAND2, RIGHTHAND2, LEFTFOOT2, RIGHTFOOT2, LEFTHAND3, RIGHTHAND3, LEFTFOOT3, RIGHTFOOT3,
               LAPTOP, TABLE
          }

          private static readonly Dictionary<LiveObjectTag, string> tagNames = new Dictionary<LiveObjectTag, string>() {
               { LiveObjectTag.HEADSET1, "VR1" },
               { LiveObjectTag.HEADSET2, "VR2" },
               { LiveObjectTag.HEADSET3, "VR3" },
               { LiveObjectTag.HEADSET4, "VR4" },
               { LiveObjectTag.WAND1, "VR1_wand" },
               { LiveObjectTag.WAND2, "VR2_wand" },
               { LiveObjectTag.WAND3, "VR3_wand" },
               { LiveObjectTag.WAND4, "VR4_wand" },
               { LiveObjectTag.BOX1, "VR1_box" },
               { LiveObjectTag.LEFTHAND1, "VR1_lefthand"},
               { LiveObjectTag.RIGHTHAND1, "VR1_righthand"},
               { LiveObjectTag.LEFTFOOT1, "VR1_leftankle"},
               { LiveObjectTag.RIGHTFOOT1, "VR1_rightankle"},
               { LiveObjectTag.LEFTHAND2, "VR2_lefthand"},
               { LiveObjectTag.RIGHTHAND2, "VR2_righthand"},
               { LiveObjectTag.LEFTFOOT2, "VR2_leftankle"},
               { LiveObjectTag.RIGHTFOOT2, "VR2_rightankle"},
               { LiveObjectTag.LEFTHAND3, "VR3_lefthand"},
               { LiveObjectTag.RIGHTHAND3, "VR3_righthand"},
               { LiveObjectTag.LEFTFOOT3, "VR3_leftankle"},
               { LiveObjectTag.RIGHTFOOT3, "VR3_rightankle"},
               { LiveObjectTag.LAPTOP, "VR1_laptop"},
               { LiveObjectTag.TABLE, "VR1_table"}
          };

          public static string GetName(LiveObjectTag tag) {
               if (tagNames.ContainsKey(tag)) {
                    return tagNames[tag];
               } else {
                    throw new System.ArgumentException("Illegal tag.");
               }
          }
     }
}

