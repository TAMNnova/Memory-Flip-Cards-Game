using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Sprite[] cardSprites;

    private List<int> cardIDList = new List<int>();
    private List<Cards> cardList = new List<Cards>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateCardID();
        ShuffleCardID();
        InitBoard();
    }

    void GenerateCardID()
    {
        for (int i = 0; i < cardSprites.Length; i++)
        {
            cardIDList.Add(i);
            cardIDList.Add(i);
        }
    }

    void ShuffleCardID()
    {
        int cardCount = cardIDList.Count;
        
        for (int i = 0; i < cardCount; i++)
        {
            int randomIndex = Random.Range(i, cardCount);
            int temp = cardIDList[randomIndex];
            cardIDList[randomIndex] = cardIDList[i];
            cardIDList[i] = temp;
        }
    }

    void InitBoard()
    {
        float spaceY = 1.6f;
        float spaceX = 1.06f;

        int rowCount = 5;
        int colCount = 4; 

        int cardIndex = 0;
        
        
        for(int row =0; row < rowCount; row++)
        {
            for(int col = 0; col < colCount; col++)
            {
                float posX = (col - (colCount/2))*spaceX + (spaceX/2);
                float posY = ((row - (int)(rowCount/2)) * spaceY) - 0.5f;

                Vector3 pos = new Vector3(posX,posY,0f);

                GameObject cardObj = Instantiate(cardPrefab,pos,Quaternion.identity);
                Cards card = cardObj.GetComponent<Cards>();

                int cardID = cardIDList[cardIndex++];
                card.SetCardID(cardID);
                card.SetCardSprite(cardSprites[cardID]);

                cardList.Add(card);
            }
        }
    }

    public List<Cards> GetCards()
    {
        return cardList;
    }

    
}
