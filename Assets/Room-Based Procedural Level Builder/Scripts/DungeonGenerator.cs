using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ProceduralDungeon
{
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject startRoom;
        public GameObject masterPrefab;
        //public GameObject closedDoorPrefab;
        public GameObject doorPrefab;

        [Header("Generation Settings")]
        [Tooltip("Each level is its own Scriptable Object and has its own generation settings and prefabs.\nRefer to the Demo Level SO for more info.")]
        public List<LevelSO> levels = new List<LevelSO>();

        [Header("Events")]
        [Tooltip("Invoked when dungeon generation starts")]
        public UnityEvent onGenerationStarted = new UnityEvent();

        [Tooltip("Invoked when dungeon generation completes")]
        public UnityEvent onGenerationCompleted = new UnityEvent();

        private LevelSO levelData;
        private float hallwayChance;
        private float sequentialHallwayChanceMulti;

        private float currentHallwayChance;
        private List<GameObject> placedRooms = new List<GameObject>();
        private GameObject roomPrefabToPlace;
        private Transform dungeonParent;
        private int maxRooms;
        private int overlapAttempts = 0;

        public bool activelyGenerating = false;

        private int generatedRooms = 0;

        private void Awake()
        {
            placedRooms.Add(startRoom);
            ResetDungeon(); // Remove if you don't want dungeons to generate on Awake()
        }

        public void ClearLevel()
        {
            placedRooms.Clear();
            placedRooms.Add(startRoom);

            foreach (Transform child in startRoom.transform)
            {
                if (child.CompareTag("Entry") && child.GetComponent<EntryPoint>() && child.GetComponent<EntryPoint>().isConnected == true)
                {
                    child.GetComponent<EntryPoint>().isConnected = false;
                }
            }

            if (dungeonParent != null)
            {
                Destroy(dungeonParent.gameObject);
                dungeonParent = null;
                StopAllCoroutines();
            }
        }
        public void ResetDungeon()
        {
            if (activelyGenerating == false)
            {
                activelyGenerating = true;
                onGenerationStarted.Invoke();
                //Debug.Log(activelyGenerating);

                ClearLevel();

                if (levels.Count > 0)
                {
                    levelData = levels[Random.Range(0, levels.Count)];
                    Debug.Log(levelData.levelName + "level selected");
                }

                SetUpLevelData(levelData);
                StartCoroutine(GenerateDungeon());
            }
            else Debug.LogWarning("Dungeon is already generating!");
        }

        void SetUpLevelData(LevelSO levelData)
        {
            maxRooms = levelData.totalRooms;
            hallwayChance = levelData.hallwayChance;
            sequentialHallwayChanceMulti = levelData.sequentialHallwayChanceMultiplier;
            currentHallwayChance = hallwayChance;
        }

        IEnumerator GenerateDungeon()
        {
            if (dungeonParent == null)
            {
                GameObject parent = new GameObject("Generated Maze");
                dungeonParent = parent.transform;
            }

            if (generatedRooms < maxRooms)
            {
                activelyGenerating = true;
                while(activelyGenerating) {
                    yield return StartCoroutine(PlaceNextRoom());
                }
            }
            else {
                Debug.Log("dfsajpfjifsjfioajoifsjaioas");
                activelyGenerating = false;
                onGenerationCompleted.Invoke();
            }

            //if(closedDoorPrefab != null)
            //{
            //    SealOpenDoors();
            //}

            //yield return new WaitForSeconds(2f);

            //Debug.Log(activelyGenerating);
        }


        IEnumerator PlaceNextRoom()
        {
            //Debug.Log(placedRooms.Count);
            if (generatedRooms >= maxRooms) {
                activelyGenerating = false;
            }

            int sourceRoomIndex;

            if (placedRooms.Count == 1) {
                sourceRoomIndex = 0;
            }
            else {
                sourceRoomIndex = Random.Range(0, placedRooms.Count);
            }

            //Debug.Log($"{sourceRoomIndex} / {placedRooms.Count}");
            GameObject sourceRoom = placedRooms[sourceRoomIndex];

            if (levelData == null)
            {
                roomPrefabToPlace = masterPrefab;
            }

            else
            {
                if (Random.value < currentHallwayChance)
                {
                    roomPrefabToPlace = levelData.hallwayPrefabs[Random.Range(0, levelData.hallwayPrefabs.Count)];
                    currentHallwayChance = hallwayChance * sequentialHallwayChanceMulti;
                }
                else
                {
                    roomPrefabToPlace = levelData.roomPrefabs[Random.Range(0, levelData.roomPrefabs.Count)];
                    currentHallwayChance = hallwayChance;
                }
            }


            List<Transform> sourceEntries = new List<Transform>();
            List<Transform> newEntries = new List<Transform>();

            sourceEntries.Clear();
            newEntries.Clear();

            foreach (Transform child in sourceRoom.transform)
            {
                if (child.CompareTag("Entry") && child.GetComponent<EntryPoint>() && child.GetComponent<EntryPoint>().isConnected == false)
                {
                    sourceEntries.Add(child);
                }
            }

            if (sourceEntries.Count == 0)
            {
                placedRooms.Remove(sourceRoom);
                yield break;
            }

            var selectedSourceEntryIndex = Random.Range(0, sourceEntries.Count);
            //Debug.Log($"{selectedSourceEntryIndex} / {sourceEntries.Count} Entries");
            Transform selectedSourceEntry = sourceEntries[selectedSourceEntryIndex];
            EntryPoint sourceEntryPoint = selectedSourceEntry.GetComponent<EntryPoint>();

            GameObject roomToSpawn = InstantiateUnderParent(roomPrefabToPlace, Vector3.zero, Quaternion.identity);

            foreach (Transform child in roomToSpawn.transform)
            {
                if (child.CompareTag("Entry") && child.GetComponent<EntryPoint>() && child.GetComponent<EntryPoint>().isConnected == false)
                {
                    newEntries.Add(child);
                }
            }

            Transform selectedNewEntry = newEntries[Random.Range(0, newEntries.Count)];
            EntryPoint newEntryPoint = selectedNewEntry.GetComponent<EntryPoint>();

            Quaternion rotation = Quaternion.FromToRotation(selectedNewEntry.forward, -selectedSourceEntry.forward);
            roomToSpawn.transform.rotation = rotation;

            Vector3 entryGlobalPos = roomToSpawn.transform.TransformPoint(selectedNewEntry.localPosition);
            Vector3 offset = selectedSourceEntry.position - entryGlobalPos;
            roomToSpawn.transform.position = offset;

            FixUpsideDown(roomToSpawn, selectedNewEntry, selectedSourceEntry);

            if (IsRoomOverlapping(roomToSpawn))
            {
                overlapAttempts++;
                Destroy(roomToSpawn);
                sourceEntryPoint.isConnected = false;

                if (overlapAttempts >= 3)
                {
                    var roomToRemove = placedRooms[placedRooms.Count - 1];
                    placedRooms.Remove(roomToRemove);
                    Destroy(roomToRemove);

                    Debug.Log("FUCKED!");
                }

                yield return null;
            }
            else
            {
                overlapAttempts = 0;

                placedRooms.Add(roomToSpawn);
                generatedRooms += 1;
                sourceEntryPoint.isConnected = true;
                newEntryPoint.isConnected = true;
                sourceEntryPoint.connectedEntry = newEntryPoint;
                newEntryPoint.connectedEntry = sourceEntryPoint;
                yield return null;
            }
        }

        void FixUpsideDown(GameObject roomToSpawn, Transform selectedEntry, Transform sourceEntry)
        {
            if (Vector3.Dot(roomToSpawn.transform.up, Vector3.down) > 0.9f)
            {
                // GameObject nicheOriginFix = new GameObject();
                // roomToSpawn.transform.SetParent(nicheOriginFix.transform);
                
                //roomToSpawn.transform.RotateAround(selectedEntry.position, selectedEntry.forward, 180f);
                roomToSpawn.transform.Rotate(selectedEntry.forward, 180f, Space.Self);

                float offset = 2 * selectedEntry.localPosition.x - roomToSpawn.transform.localPosition.x;
                Debug.Log(offset);
                roomToSpawn.transform.localPosition += new Vector3(offset,0,0);

                // roomToSpawn.transform.SetParent(dungeonParent);
                // Destroy(nicheOriginFix);
            }
        }
        private bool IsRoomOverlapping(GameObject room)
        {
            Collider collider = room.GetComponent<Collider>();

            if(collider == null)
            {
                Debug.LogError("Room " + room.name + " has no collider! Add a collider to prevent overlapping.");
                return true;
            }

            LayerMask roomLayer = 1 << LayerMask.NameToLayer("Rooms");

            BoxCollider boxCollider = collider as BoxCollider;
            if (boxCollider != null)
            {
                Vector3 worldSize = Vector3.Scale(boxCollider.size, room.transform.lossyScale);

                Collider[] hitColliders = Physics.OverlapBox(
                    room.transform.position + Vector3.Scale(boxCollider.center, room.transform.lossyScale),
                    worldSize * 0.49f, // smaller to avoid self-detection
                    room.transform.rotation,
                    roomLayer,
                    QueryTriggerInteraction.Collide
                );

                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject != room)
                    {
                        Debug.Log($"Collision detected: {room.name} overlaps with {hitCollider.gameObject.name}");
                        return true;
                    }
                }

                return false;
            }
            return false;
        }
        void SealOpenDoors()
        {
            foreach (GameObject room in placedRooms)
            {
                foreach (Transform child in room.transform)
                {
                    if (child.CompareTag("Entry") && child.GetComponent<EntryPoint>())
                    {
                        EntryPoint entry = child.GetComponent<EntryPoint>();
                        if (!entry.isConnected) {
                            bool hasOverlappingEntry = false;
                            float checkRadius = 0.1f;

                            Collider[] colliders = Physics.OverlapSphere(child.position, checkRadius, ~0, QueryTriggerInteraction.Collide);
                            foreach (Collider col in colliders)
                            {
                                if (col.transform == child) continue;

                                if (col.CompareTag("Entry") && col.GetComponent<EntryPoint>())
                                {
                                    hasOverlappingEntry = true;
                                    break;
                                }
                            }

                            if (!hasOverlappingEntry)
                            {
                                //GameObject door = InstantiateUnderParent(closedDoorPrefab, child.transform.position, child.transform.rotation);
                            }
                        }

                        //if (entry.isConnected && entry.connectedEntry != null && entry.doorAble)
                        //{
                        //    if (entry.GetInstanceID() < entry.connectedEntry.GetInstanceID())
                        //    {
                        //        GameObject door = InstantiateUnderParent(doorPrefab, child.position, child.rotation);
                        //    }
                        //}

                        Destroy(child.gameObject);
                    }
                }
            }
        }

        private GameObject InstantiateUnderParent(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject obj = Instantiate(prefab, position, rotation);
            obj.transform.SetParent(dungeonParent);
            return obj;
        }
    }
}

