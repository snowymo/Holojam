using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using ProtoBuf;
using update_protocol_v3;

namespace Holojam {
     public enum LiveObjectTag {
          HEADSET1, HEADSET2, HEADSET3, HEADSET4, WAND1, WAND2, WAND3, WAND4
     }

     public class MasterStream : Singleton<MasterStream> {
          /////Readonly/////
          public static readonly Vector3 DEFAULT_VECTOR_POSITION = new Vector3(0, 0, 0);
          public static readonly Quaternion DEFAULT_QUATERNION_ROTATION = new Quaternion(0, 0, 0, 0);
          private const int PACKET_SIZE = 65507; // ~65KB buffer sizes
          private const int BLACK_BOX_CLIENT_PORT = 1611;

          private static readonly Dictionary<LiveObjectTag, string> tagToMotiveName = new Dictionary<LiveObjectTag, string>() {
               { LiveObjectTag.HEADSET1, "VR1" },
               { LiveObjectTag.HEADSET2, "VR2" },
               { LiveObjectTag.HEADSET3, "VR3" },
               { LiveObjectTag.HEADSET4, "VR4" },
               { LiveObjectTag.WAND1, "VR1_wand" },
               { LiveObjectTag.WAND2, "VR2_wand" },
               { LiveObjectTag.WAND3, "VR3_wand" },
               { LiveObjectTag.WAND4, "VR4_wand" }
          };

          /////Private/////
          //References
          private Dictionary<string, LiveObjectStorage> liveObjects = new Dictionary<string, LiveObjectStorage>();
          private UnityEngine.Object lockObject = new UnityEngine.Object();
          private PacketBuffer previousPacket = new PacketBuffer(PACKET_SIZE);
          private PacketBuffer currentPacket = new PacketBuffer(PACKET_SIZE);
          private update_protocol_v3.Update update;
          //Primitives
          private int packetCount = 0;
          private bool receivingPackets = true;

          //private class OtherMarker
          //{
          //	public Vector3 position;
          //}

          private class LiveObjectStorage {
               public Vector3 position;
               public Quaternion rotation;
               public int buttonBits;
               public List<Vector2> axisButtons;
          }

          private class PacketBuffer {

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

          ///////////////////////////////////////////////////////////////////////////
          //
          // Inherited from MonoBehaviour
          //

          private void Start() {
               Application.runInBackground = true;
               Application.targetFrameRate = -1;

               System.Threading.Thread thread = new System.Threading.Thread(run);
               thread.Start();
               StartCoroutine(DisplayPacketsPerSecond());
          }

          ///////////////////////////////////////////////////////////////////////////
          //
          // Debug functions
          //

          private IEnumerator DisplayPacketsPerSecond() {
               while (receivingPackets) {
                    yield return new WaitForSeconds(1f);
                    Debug.LogWarning(string.Format("Packets per second: {0}", packetCount));
                    packetCount = 0;
               }
          }

          ///////////////////////////////////////////////////////////////////////////
          //
          // API functions
          //

          public bool GetPosition(LiveObjectTag tag, out Vector3 position) {
               if (!tagToMotiveName.ContainsKey(tag)) {
                    throw new System.ArgumentException("Illegal tag.");
               }
               LiveObjectStorage storage;
               lock (lockObject) { 
                    if (!liveObjects.TryGetValue(tagToMotiveName[tag], out storage)) {
                         position = DEFAULT_VECTOR_POSITION;
                         return false;
                    } else {
                         position = storage.position;
                         return true;
                    }
               }
          }

          public bool GetRotation(LiveObjectTag tag, out Quaternion rotation) {
               if (!tagToMotiveName.ContainsKey(tag)) {
                    throw new System.ArgumentException("Illegal tag.");
               }
               LiveObjectStorage storage;
               lock (lockObject) {
                    if(!liveObjects.TryGetValue(tagToMotiveName[tag], out storage)) {
                         rotation = DEFAULT_QUATERNION_ROTATION;
                         return false;
                    } else {
                         rotation = storage.rotation;
                         return true;
                    }
               }
          }

          public Vector3 getLiveObjectPosition(string name) {
               LiveObjectStorage storage;
               lock (lockObject) {
                    if (!liveObjects.TryGetValue(name, out storage)) {
                         return DEFAULT_VECTOR_POSITION;
                    }
               }
               return storage.position;
          }

          public Quaternion getLiveObjectRotation(string name) {
               LiveObjectStorage storage;
               lock (lockObject) {
                    if (!liveObjects.TryGetValue(name, out storage)) {
                         return DEFAULT_QUATERNION_ROTATION;
                    }
               }
               return storage.rotation;
          }

          public int getLiveObjectButtonBits(string name) {
               LiveObjectStorage storage;
               lock (lockObject) {
                    if (!liveObjects.TryGetValue(name, out storage)) {
                         return 0;
                    }
               }
               return storage.buttonBits;
          }

          public Vector2 getLiveObjectAxisButton(string name, int index) {
               LiveObjectStorage storage;
               lock (lockObject) {
                    if (!liveObjects.TryGetValue(name, out storage)) {
                         //print ("Body not found: " + name);
                         return Vector2.zero;
                    }
               }
               return storage.axisButtons[index];
          }

          ///////////////////////////////////////////////////////////////////////////
          //
          // Thread methods
          //

          // This thread handles incoming NatNet packets.
          private void run() {
               Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
               socket.Bind(new IPEndPoint(IPAddress.Any, BLACK_BOX_CLIENT_PORT));
               socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse("224.1.1.1")));

               int nBytesReceived = 0;
               while (receivingPackets) {
                    nBytesReceived = socket.Receive(currentPacket.bytes);
                    currentPacket.stream.Position = 0;

                    update = Serializer.Deserialize<update_protocol_v3.Update>(new MemoryStream(currentPacket.bytes, 0, nBytesReceived));

                    currentPacket.frame = update.mod_version;
                    if (currentPacket.frame > previousPacket.frame) {
                         packetCount++;

                         currentPacket.stream.Position = 0;
                         previousPacket.copyFrom(currentPacket);

                         for (int j = 0; j < update.live_objects.Count; j++) {
                              LiveObject or = update.live_objects[j];
                              string label = or.label;

                              LiveObjectStorage ow;
                              lock (lockObject) {
                                   if (!liveObjects.TryGetValue(label, out ow)) {
                                        ow = new LiveObjectStorage();
                                        liveObjects[label] = ow;
                                   } else {
                                        ow = liveObjects[label];
                                   }
                                   if (update.lhs_frame) {
                                        ow.position = new Vector3(-(float)or.x, (float)or.y, (float)or.z);
                                        ow.rotation = new Quaternion(-(float)or.qx, (float)or.qy, (float)or.qz, -(float)or.qw);
                                   } else {
                                        ow.position = new Vector3((float)or.x, (float)or.y, (float)or.z);
                                        ow.rotation = new Quaternion((float)or.qx, (float)or.qy, (float)or.qz, (float)or.qw);
                                   }
                                   ow.buttonBits = or.button_bits;
                              }
                         }
                    }

                    if (!receivingPackets) {
                         socket.Close();
                    }
               }
          }

          void OnApplicationQuit() {
               this.receivingPackets = false;
          }
     }
}