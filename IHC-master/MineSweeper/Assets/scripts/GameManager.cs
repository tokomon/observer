using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    const int fijo = 5;//tamaño de los tableros
    int x = fijo;
    int y = fijo;
    int posX = 0, posY = 0;
    List<MJson> obj;
    //lista que lee el JSON
    //hacer 3d
    double[,] matrix = new double[fijo, fijo];//matriz que se debe de dibujar
    public Cell prefab;

    public Sprite vanilaSprite;//valor no descubierto
    public Sprite mineSprite;//mina
    public Sprite freeSprite;//espacio presionado
    public Sprite paredSprite;//espacio presionado



    void Start()
    {
        TableroInstance(posX, posY);
        TableroInstance(posX - (fijo + 1), posY - (fijo + 1));
        TableroInstance(posX, posY - (fijo + 1));
        TableroInstance(posX - (fijo + 1), posY);
        //todos los tableros empiezan en 0.0 de la esquina izq de abajo
        //se tendria que cambiar para que empiecen en cada esquina de los cuadrados
        //es decir (0,0) (n,n) (n,n) (n,n)
    }

    //Lista que se obtiene de leer el json
    public class MJson
    {
        public int row { get; set; }
        public int col { get; set; }
        public int content { get; set; }
    }

    public class Tablero
    {
        public int posX, posY;
        public double[,] matrix; //matriz que se debe copiar
        public Cell[,] cellMatrix;

        public Sprite free;
        public Sprite bomb;
        public Sprite wall;
        public Sprite full;

        //para poder usar los sprites definidos arriba
        public void changeSpriteFree(Sprite free_)
        {   free = free_;}
        public void changeSpriteBomb(Sprite bomb_)
        {   bomb = bomb_;}
        public void changeSpriteFull(Sprite full_)
        {   full = full_;}
        public void changeSpriteWall(Sprite wall_)
        {   wall = wall_;}
        /****************************************/
        public Tablero(int x, int y, Sprite mine, Sprite full, Sprite free, Sprite wall)
        {
            posX = x;
            posY = y;
            changeSpriteFree(free);
            changeSpriteBomb(mine);
            changeSpriteFull(full);
            changeSpriteWall(wall);

        }

        public void init()
        { //dibuja la matriz desde el inicio
            CellMatrixLoop((i, j) =>
            {
                cellMatrix[i, j].Init(new Vector2Int(i, j),
                (matrix[i, j] != (-1) ? false : true),
                Activate);
                cellMatrix[i, j].sprite = full;
            });
        }

        //funcion para onclick
        void Activate(int i, int j)
        {
            if (cellMatrix[i, j].showed)
                return;
            cellMatrix[i, j].showed = true;

            if (cellMatrix[i, j].mine)
            {
                //acaba el juego
                cellMatrix[i, j].sprite = wall;
                Debug.Log("Pared");
                //StartCoroutine(your_timer()); //Delay
            //    init();
                //volver a jugar
            }
            else
            {
                cellMatrix[i, j].sprite = free;
       

            }
        }
        void ActivateArround(int i, int j)
        {
            if (PointIsInsideMatrix(i + 1, j))
                Activate(i + 1, j);
            if (PointIsInsideMatrix(i, j + 1))
                Activate(i, j + 1);
            if (PointIsInsideMatrix(i + 1, j + 1))
                Activate(i + 1, j + 1);
            if (PointIsInsideMatrix(i - 1, j))
                Activate(i - 1, j);
            if (PointIsInsideMatrix(i, j - 1))
                Activate(i, j - 1);
            if (PointIsInsideMatrix(i - 1, j - 1))
                Activate(i - 1, j - 1);
            if (PointIsInsideMatrix(i - 1, j + 1))
                Activate(i - 1, j + 1);
            if (PointIsInsideMatrix(i + 1, j - 1))
                Activate(i + 1, j - 1);
        }
        public void CellMatrixLoop(Action<int, int> e)//recibir en un parametro una funcion
        {
            for (int i = 0; i < cellMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < cellMatrix.GetLength(1); j++)
                {
                    e(i, j);//esta funcion dibujara el cuadrado
                }
            }
        }
        bool PointIsInsideMatrix(int i, int j)
        {
            if (i >= cellMatrix.GetLength(0))
                return false;
            if (i < 0)
                return false;
            if (j >= cellMatrix.GetLength(1))
                return false;
            if (j < 0)
                return false;

            return true;
        }
    }

    void jsonToMatriz()
    {
        string json = @"[{'row': 0, 'col': 0, 'content': 1}, {'row': 0, 'col': 1, 'content': 1}, 
    	{'row': 0, 'col': 2, 'content': 3}, {'row': 0, 'col': 3, 'content': -1}, 
    	{'row': 0, 'col': 4, 'content': 2}, {'row': 1, 'col': 0, 'content': 2}, 
    	{'row': 1, 'col': 1, 'content': -1}, {'row': 1, 'col': 2, 'content': 4}, 
    	{'row': 1, 'col': 3, 'content': -1}, {'row': 1, 'col': 4, 'content': 2}, 
    	{'row': 2, 'col': 0, 'content': 3}, {'row': 2, 'col': 1, 'content': -1}, 
    	{'row': 2, 'col': 2, 'content': 6}, {'row': 2, 'col': 3, 'content': 3}, 
    	{'row': 2, 'col': 4, 'content': 2}, {'row': 3, 'col': 0, 'content': 3}, 
    	{'row': 3, 'col': 1, 'content': -1}, {'row': 3, 'col': 2, 'content': -1}, 
    	{'row': 3, 'col': 3, 'content': -1}, {'row': 3, 'col': 4, 'content': 1}, 
    	{'row': 4, 'col': 0, 'content': 2}, {'row': 4, 'col': 1, 'content': -1}, 
    	{'row': 4, 'col': 2, 'content': 4}, {'row': 4, 'col': 3, 'content': 2}, 
    	{'row': 4, 'col': 4, 'content': 1}]";


        obj = JsonConvert.DeserializeObject<List<MJson>>(json);

        for (int i = 0; i < fijo * fijo; i++)
        {
            matrix[obj[i].row, obj[i].col] = obj[i].content;
        }
    }


    void TableroInstance(int posx, int posy)
    {
        Tablero matriz = new Tablero(posx, posy, mineSprite, vanilaSprite, freeSprite, paredSprite);
        jsonToMatriz();//llenamos la matriz con la data del json
        matriz.matrix = matrix;

        if (matriz.cellMatrix == null)
        {
            matriz.cellMatrix = new Cell[x, y];//crea un objeto matriz con 2 dimensiones
            matriz.CellMatrixLoop((i, j) =>
            {
                Cell go = Instantiate(prefab,
                    new Vector3(i + posx, j + posy),
                    Quaternion.identity,
                    transform);
                //hace una copia de un objeto y ponerlo en otro lugar 
                go.name = string.Format("(X:{0},Y:{1})", i, j);

                matriz.cellMatrix[i, j] = go;
            });
        }
        matriz.init();

    }
}