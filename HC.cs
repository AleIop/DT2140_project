using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;


public class HC:MonoBehaviour {
    private int id; //id number for HC
    public List<int> myExcitatory; //pyramid cells ids within one HC
    public List<int> myInhibitory; //basket cells ids within one HC
    public List<Spike> mySpikes;
    public Dictionary<int, List<int>> activations; //associates an int to a list of ids belonging to one or more pyramids

    void Awake() {
        myExcitatory = new List<int>();
        myInhibitory = new List<int>();
        mySpikes = new List<Spike>();
        testingMe();
    }

    /*
        property associated to the variable id
    */
    public int ID {
        get {
            return this.id;
        }
        set {
            this.id = value;
        }
    }

    /*
        this method sorts and associates pyramid cells with their activations in the dictionary
        it is called in Awake(), after all lists have been instantiated
    */
    public void testingMe() {
        foreach(int index in myExcitatory) {
            if(index == DataReader.eSpikesIndex[0]) { //first element of the list to be added to the dictionary
                activations.Add(DataReader.eSpikesTimes[0], new List<int>{DataReader.eSpikesIndex[0]});
            }

            for (int i = 1; i < DataReader.eSpikesTimes.Count; i++) { //all other elements
                if (index == DataReader.eSpikesIndex[i]) {
                    if (DataReader.eSpikesTimes[i] == DataReader.eSpikesTimes[i - 1]) { //in case of same timestamps
                        List<int> temp = activations[DataReader.eSpikesTimes[i - 1]]; //list with only the id of previous pyramid with the same timestamp
                        temp.Add(DataReader.eSpikesIndex[i]); //adds the index of the second pyramid to the same list
                        activations[DataReader.eSpikesTimes[i - 1]] = temp; //replaces old list with new one
                    } else { //in case of different timestamps
                        activations.Add(DataReader.eSpikesTimes[i], new List<int>{DataReader.eSpikesIndex[i]});
                    }
                }
            }
        }
       // File.WriteAllText(@"./hcactivations.json", JsonConvert.SerializeObject(activations));
    }




    /* 
    I don't know what this method is supposed to do exactly, but it creates new spikes for some reason 

    public void LoopThisShit(){
        for(int i=0; i<DataReader.e_newSize; i++){
            //print(i);
            //print(CreateNeurons.listNE[DataReader.e_new[0]-1].GetComponent<Excitatory>().id + " " + DataReader.e_newTimes[i] );
            //mySpikes.Add(new Spike{neuronID = (CreateNeurons.listNE[DataReader.e_new[i]-1].GetComponent<Excitatory>().id), time= DataReader.e_newTimes[i]});
        }
    }
    */

}

/*
    class representing the activation of a cell, with an id and a timestamp as attributes
*/
public class Spike {
    public int neuronID{get;set;}
    public float time{get;set;}
}