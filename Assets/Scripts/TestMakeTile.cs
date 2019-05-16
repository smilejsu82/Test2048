using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TestMakeTile : MonoBehaviour
{
    public enum eDirection
    {
        None = -1,
        Up,
        Down,
        Left,
        Right
    }
    private const string tilePrefix = "Brick_wall_0";
    private int[,] arrMap;
    private int tileWidth = 128;
    private int tileHeight = 128;
    public int col = 4;
    public int row = 4;
    public Button[] buttons;
    private List<Block> blockList = new List<Block>();

    private Block selectedBlock;

    private void Awake()
    {
        this.arrMap = new int[this.col, this.row];
    }

    void Start()
    {
        this.LoadMap();
        this.CreateMap();
        var block = this.CreateBlock(0, 0);
        this.selectedBlock = block;
        //this.CreateBlock(3, 0);

        for (int i = 0; i < this.buttons.Length; i++)
        {
            var capturedIdx = i;
            this.buttons[capturedIdx].onClick.AddListener(() => {
                Debug.Log((eDirection)capturedIdx);
                var dir = (eDirection)capturedIdx;
                var finalCoord = this.GetFinalCoord(dir);

                //finalCoord ---->   2,0
                //screenPos ---->  2 * 128 + 512 - 192 ,0
                //worldPos
                var screenPos = new Vector2(finalCoord.x * 128 + 512 - 192, finalCoord.y + this.tileHeight * 4);
                var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                worldPos.z = 0;
                this.selectedBlock.coord = finalCoord;
                this.selectedBlock.transform.position = worldPos;
            });
        }
    }

    private Block CreateBlock(int initX, int initY)
    {
        var tileId = 2;
        var prefabName = TestMakeTile.tilePrefix + tileId;
        var loadedPrefab = Resources.Load<GameObject>(prefabName);
        var tileGo = Instantiate(loadedPrefab);
        var screenPos = new Vector2(initX * this.tileWidth + 512 - 192, initY * -this.tileHeight + this.tileHeight * 4);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        tileGo.transform.position = worldPos;
        var block = tileGo.GetComponent<Block>();
        block.coord = new Vector2(initX, initY);
        this.blockList.Add(block);
        return block;
    }

    private void CreateMap()
    {
        for (int i = 0; i < this.col; i++)
        {
            for (int j = 0; j < this.row; j++)
            {
                var tileId = this.arrMap[i, j];
                var prefabName = TestMakeTile.tilePrefix + tileId;
                var loadedPrefab = Resources.Load<GameObject>(prefabName);
                var tileGo = Instantiate(loadedPrefab); 
                var screenPos = new Vector2(j * this.tileWidth + 512 - 192, i * -this.tileHeight + this.tileHeight * 4);
                var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                worldPos.z = 0;
                tileGo.transform.position = worldPos;
                tileGo.GetComponent<Tile>().text.text = string.Format("{0},{1}", j, i);
                tileGo.GetComponent<Tile>().coord = new Vector2(j, i);
            }
            Debug.Log("\n");
        }
    }

    private void LoadMap()
    {
        var textAsset = Resources.Load<TextAsset>("stage_01");
        Debug.LogFormat("textAsset: {0}", textAsset);

        var arrStr = textAsset.text.Split(',', '\n');
        int idx = 0;

        for (int i = 0; i < this.col; i++)
        {
            for (int j = 0; j < this.row; j++, idx++)
            {
                //FormatException: Input string was not in a correct format.
                var str = arrStr[idx];

                Debug.Log("---> " + str);

                var val = 0;
                var isParsed = int.TryParse(str, out val);
                if (isParsed)
                {
                    arrMap[i, j] = val;
                }
                else
                {
                    Debug.Log("---> " + isParsed + " , " + val);
                }
            }
        }
    }

    private Vector2 GetFinalCoord(eDirection dir)
    {
        //오른쪽 
        //this.selectedBlock
        //현재 블록의 좌표 
        Debug.LogFormat("coord: {0}", this.selectedBlock.coord);
        var targetCoord = new Vector2(this.col - 1, selectedBlock.coord.y);

        for (int i = (int)this.selectedBlock.coord.x; i < col; i++)
        {
            var searchCoord = new Vector2(i, this.selectedBlock.coord.y);
            var foundBlock = this.blockList.Find(x => x.coord == searchCoord);
            Debug.LogFormat("foundBlock: {0}", foundBlock);
            if (foundBlock!=null && foundBlock != this.selectedBlock)
            {
                targetCoord = new Vector2(foundBlock.coord.x-1, foundBlock.coord.y);
                break;
            }
        }

        return targetCoord;
    }

}
