using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;


public class HC:MonoBehaviour{
		private int id; // id number for HC
		public List<int> myExcitatory;
		public List<int> myInhibitory;
		public List<Spike> mySpikes;
        public Dictionary<int, List<int>> activations;

		void Awake()
		{
			myExcitatory = new List<int>();
			myInhibitory = new List<int>();
			mySpikes = new List<Spike>();
            testingMe();
        }

		public int ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

    public void testingMe()
    {
        foreach( int index in myExcitatory)
        {
            if(index == DataReader.eSpikesIndex[0])
            {
                activations.Add(DataReader.eSpikesTimes[0], new List<int> { DataReader.eSpikesIndex[0] });
            }
            for (int i = 1; i < DataReader.eSpikesTimes.Count; i++)
            {
                if (index == DataReader.eSpikesIndex[i])
                {

                    if (DataReader.eSpikesTimes[i] == DataReader.eSpikesTimes[i - 1])
                    {
                        List<int> temp = activations[DataReader.eSpikesTimes[i - 1]];
                        temp.Add(DataReader.eSpikesIndex[i]);
                        activations[DataReader.eSpikesTimes[i - 1]] = temp;
                    }
                    else
                    {
                        activations.Add(DataReader.eSpikesTimes[i], new List<int> { DataReader.eSpikesIndex[i] });
                    }
                }
            }
        }

       // File.WriteAllText(@"./hcactivations.json", JsonConvert.SerializeObject(activations));

    }




    /* 
            public void LoopThisShit(){
                for(int i=0; i<DataReader.e_newSize; i++){
                    //print(i);
                    //print(CreateNeurons.listNE[DataReader.e_new[0]-1].GetComponent<Excitatory>().id + " " + DataReader.e_newTimes[i] );
                    //mySpikes.Add(new Spike{neuronID = (CreateNeurons.listNE[DataReader.e_new[i]-1].GetComponent<Excitatory>().id), time= DataReader.e_newTimes[i]});
                }
            }*/

}
	public class Spike{
		public int neuronID{get;set;}
		public float time{get;set;}
	}