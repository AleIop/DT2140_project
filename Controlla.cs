using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controlla : MonoBehaviour {

public CreateNeurons myCreateNeuron;
public DataReader myDataReader;
public UIActions myUIActions;


// This class instantiates everything in the right order, first data, then neurons and then UIActions

	void Start(){
		myDataReader.Initiate();
		myCreateNeuron.Initiate();
		myCreateNeuron.Create();
		myUIActions.Initiate();
	}

}
