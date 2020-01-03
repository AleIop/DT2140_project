using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

public class DataReader:MonoBehaviour{
	public static ParamData myPD;
	public static ModelData myMD;
	public static SpikesData mySD;
	public static ProgramData[] myPrD;

    public float pyramidDuration;
    public float basketDuration;

	//ParamData
	public static List<List<int>> stim_matrix_default;
	public static int n_HC; 				//number of HCs
	public static int row_HC; 				//number of HCs in each row
	public static  int col_HC; 				//number of HCs in each column
	public static int n_MC_per_HC; 			//number of MCs per HC
	public static  int n_MC; 				//number of MCs
	public static  int row_MC; 				//number of MCs in each row
	public static  int col_MC; 				//number of MCs in each column
	public static int nrn_per_MC;			//number of neurons per MC
	public static int n_neurons; 			//number of neurons
	public static int n_exc; 				//number of pyramid cells
	public static int n_inh; 				//number of basket cells
	public static int basket_per_MC; 		//number of basket cells per MC
	public static List<List<int>> MC_HC; 	//association of MCs for every HC

	//ModelData
	public static List<List<int>> GridPosition_MC; 	//position of an MC in the physical displacement matrix
	public static List<List<int>> e_pop_HC;
	public static List<List<int>> i_pop_HC;
	public static List<List<int>> e_rec_MC; 		//association of pyramid cells with recorded times

	//Spikes
	public static List<int> i_src;			//list of basket cell index, connected with i_times
	public static List<float> i_times;		//list of basket cell times, connected with i_src
	public static List<int> e_src;			//list of pyramid cell index, connected with e_times
	public static List<float> e_times;		//list of pyramid cell times, connected with e_src
	public static List<float> i_newTimes;	//list of basket cells sorted by time
	public static List<float> i_newTimes2;	//list of basket cells sorted by time
	public static List<float> e_newTimes;	//list of pyramid cells sorted by time
	public static List<int> idx_iNewt;		//list of sorted index for time
	public static List<int> idx_eNewt;		//list of sorted index for time
	public static List<int> e_new; 			// THIS ONE is like e_src but sorted by time! Use it if you check by index
	public static List<int> i_new;          // THIS ONE is like i_src but sorted by time! Use it if you check by index


    public static List<int> eSpikesTimes;
    public static List<int> eSpikesIndex;

	//distinct dictionaries for pyramid and basket cells active and inactive phases
    public static Dictionary<int, List<int>> phase;
    public static Dictionary<int, List<int>> phaseDeactivation;
    public static Dictionary<int, List<int>> basketphase;
    public static Dictionary<int, List<int>> basketDeactivation;


    //ProgramData
    // this is the stimulation phase labels!
    public static List<float> myStop;
	public static List<string> myNames;
	public static List<float> myStart;

	/*
		this method instantiates the parameter, model, spikes and program data, the label lists and the dictionaries and then calls all the other methods
		it is called in Controlla.cs, before anything else
	*/
	public void Initiate(){
		myPD = new ParamData().Initiate();
		myMD = new ModelData().Initiate();
		mySD = new SpikesData().Initiate();
		myPrD = new ProgramData().Initiate();
		//ActivityHC = new List<List<Spike>>();
		myStop = new List<float>();
		myNames = new List<string>();
		myStart = new List<float>();
        phase = new Dictionary<int, List<int>>();
        phaseDeactivation = new Dictionary<int, List<int>>();
        basketDeactivation = new Dictionary<int, List<int>>();
        basketphase = new Dictionary<int, List<int>>();

		ReformateData();
		SetValues();
		CreatePhaseList();

		//File.WriteAllText(@"./id_e_rec.json", JsonConvert.SerializeObject(e_rec_MC));
		//File.WriteAllText(@"./id_i_pop.json", JsonConvert.SerializeObject(i_pop_HC));
		//File.WriteAllText(@"./eSource.json", JsonConvert.SerializeObject(e_src));
	}

	/*
		This method sorts the data based on time, transforming it into (guess what) lists
		it is called in this.Initiate(), after instantiating all the variables
	*/
	private void ReformateData(){
		var iTimesSorted = mySD.i_rec.sp_times
		    .Select((x, i) => new KeyValuePair<float,int>(x, i))
		    .OrderBy(x => x.Key)
		    .ToList();
		var eTimesSorted = mySD.e_rec.sp_times
				.Select((x, i) => new KeyValuePair<float,int>(x, i))
				.OrderBy(x => x.Key)
				.ToList();
		i_newTimes = iTimesSorted.Select(x => x.Key).ToList();
		idx_iNewt = iTimesSorted.Select(x => x.Value).ToList();
		e_newTimes = eTimesSorted.Select(x => x.Key).ToList();
		idx_eNewt = eTimesSorted.Select(x => x.Value).ToList();
	}

	/*
		This method creates "shortcuts" for the variables stored in the json file, and adds them to the various lists
		it is called in this.Initiate(), after this.ReformateData()
	*/
	private void SetValues(){
		e_new = new List<int>();
		i_new = new List<int>();
		i_newTimes2 = new List<float>();
		n_HC = myPD.n_HC;
		row_HC = myPD.row_HC;
		col_HC = myPD.col_HC;
		n_MC_per_HC = myPD.n_MC_per_HC;
		n_MC = myPD.n_MC;
		row_MC = myPD.row_MC;
		col_MC = myPD.col_MC;
		nrn_per_MC = myPD.nrn_per_MC;
		n_neurons = myPD.n_neurons;
		n_exc = myPD.n_exc;
		n_inh = myPD.n_inh;
		MC_HC = myPD.MC_HC;
		basket_per_MC = myPD.basket_per_MC;
		e_rec_MC = myMD.e_rec_MC;
		e_pop_HC = myMD.e_pop_HC;
		GridPosition_MC = myMD.GridPosition_MC;
		i_pop_HC = myMD.i_pop_HC;
		i_src = mySD.i_rec.sp_src;
		i_times = mySD.i_rec.sp_times;
		e_src = mySD.e_rec.sp_src;
		e_times = mySD.e_rec.sp_times;

		foreach(var item in idx_eNewt){
			e_new.Add(e_src[item]);
		}	
		foreach(var item in idx_iNewt){
			i_new.Add(i_src[item]);
		}
		foreach (var item in i_newTimes){
			i_newTimes2.Add(Mathf.Round(item));
		}
		for (int i=0; i<myPrD.Count(); i++){
			myStop.Add(myPrD[i].stop);
			myNames.Add(myPrD[i].name);
			myStart.Add(myPrD[i].start);
		//print(e_src.Count()+ " enew count " + e_new.Count());
		}
	}

	/*
		this method makes the inhibitory and excitatory spikes into separate lists and then separates their components into other lists
		it is called in this.Initiate(), after this.SetValues()
	*/
	private void CreatePhaseList(){
		var eSpikes = e_times
			.Select((f, index) => new {Time = Mathf.RoundToInt(f), Index = index})
			.Where(r => r.Index != -1)
			.OrderBy(r => r.Time)
			.ToList();

		var iSpikes = i_times
			.Select((f, index) => new {Time = Mathf.RoundToInt(f), Index = index})
			.Where(r => r.Index != -1)
			.OrderBy(r => r.Time)
			.ToList();
			//add this if you want specified time .Where(r => (r.Time >=myStart[0] && r.Time<myStart[1]))

		
		eSpikesTimes = eSpikes.Select(x => x.Time).ToList();
		eSpikesIndex = eSpikes.Select(x => x.Index).ToList();
		List<int> iSpikesTimes = iSpikes.Select(x => x.Time).ToList();
		List<int> iSpikesIndex = iSpikes.Select(x => x.Index).ToList();

		//phase.Add(eSpikesTimes[0],new int[] {eSpikesIndex[0]});
		//at the beginning, adds all active cells to the respective active dictionaries and does the same for the inactive ones
		if(phase.Count()<1){
			phase.Add(eSpikesTimes[0], new List<int>{eSpikesIndex[0]});
			basketphase.Add(iSpikesTimes[0], new List<int>{iSpikesIndex[0]});
            phaseDeactivation.Add((eSpikesTimes[0]+25), new List<int>{eSpikesIndex[0]});
            basketDeactivation.Add((iSpikesTimes[0]+25), new List<int>{iSpikesIndex[0]});
        }

		//for both pyramid and basket cells adds all activation and deactivation times to the lists, while compressing repeated values into the same list
        for (int i =1 ; i<eSpikesTimes.Count() ; i++) {
			if(eSpikesTimes[i]==eSpikesTimes[i-1]) {
				List<int> temp = phase[eSpikesTimes[i-1]];
				temp.Add(eSpikesIndex[i]);
				phase[eSpikesTimes[i-1]] = temp;
                phaseDeactivation[eSpikesTimes[i-1]+25] = temp;
			} else {
				phase.Add(eSpikesTimes[i], new List<int>{eSpikesIndex[i]});
                phaseDeactivation.Add(eSpikesTimes[i]+25, new List<int>{eSpikesIndex[i]});
            }
		}
		for(int i =1 ; i<iSpikesTimes.Count() ; i++){
			if(iSpikesTimes[i]==iSpikesTimes[i-1]){
				List<int> temp = basketphase[iSpikesTimes[i-1]];
				temp.Add(iSpikesIndex[i]);
				basketphase[iSpikesTimes[i-1]] = temp;
                basketDeactivation[iSpikesTimes[i-1]+25] = temp;
			} else {
				basketphase.Add(iSpikesTimes[i], new List<int>{iSpikesIndex[i]});
                basketDeactivation.Add(iSpikesTimes[i]+25, new List<int>{iSpikesIndex[i]});
            }
        }
		//File.WriteAllText(@"./basket.json", JsonConvert.SerializeObject(basketphase));
	}

	//Fetch necessary data from params_VMDG_0
	public class ParamData {
		//properties associated to the variables declared at the beginning (lines 18-31)
		public List<List<int>> stim_matrix_default{get; set;}
		public int n_HC{get; set;}
		public int row_HC{get; set;}
		public int col_HC{get; set;}
		public int n_MC_per_HC{get; set;}
		public int n_MC{get; set;}
		public int row_MC{get; set;}
		public int col_MC {get; set;}
		public int nrn_per_MC{get; set;}
		public int n_neurons{get; set;}
		public int n_exc{get; set;}
		public int n_inh{get; set;}
		public int basket_per_MC{get; set;}
		public List<List<int>> MC_HC{get; set;}

		/*
			this method reads parameter data from a json file and converts it to multiple attribute-value tuples, before returning it
			it is called in DataReader.Initiate(), before anything else
		*/
		public ParamData Initiate(){
			string path = Application.dataPath + "/JsonData/params_VMDG_0.json";
			string jsonString = File.ReadAllText(path);
			ParamData pd= JsonConvert.DeserializeObject<ParamData>(jsonString);
			return pd;
		}
	}

	//Fetch necessary data from model_VMDG_0
	public class ModelData {
		//properties associated to the variables declared at the beginning (lines 34-37)
		public List<List<int>> GridPosition_MC{get; set;}
		public List<List<int>> e_pop_HC{get; set;}
		public List<List<int>> i_pop_HC{get; set;}
		public List<List<int>> e_rec_MC{get; set;}

		/*
			this method reads model data from a json file and converts it to multiple attribute-value tuples, before returning it
			it is called in DataReader.Initiate(), after ParamData.Initiate()
		*/
		public ModelData Initiate(){
			string path = Application.dataPath + "/JsonData/model_VMDG_0.json";
			string jsonString = File.ReadAllText (path);
			ModelData md= JsonConvert.DeserializeObject<ModelData>(jsonString);
			return md;
		}
	}

	//Fetch necessary data from spikes_VMDG_0
	public class SpikesData {
		public I_Rec i_rec{get; set;}
		public E_Rec e_rec{get; set;}

		/*
			this method reads spike data from a json file and converts it to multiple attribute-value tuples, before returning it
			it is called in DataReader.Initiate(), after ModelData.Initiate()
		*/
		public SpikesData Initiate(){
			//string path = Application.dataPath + "/JsonData/spikestest.json";
			string path = Application.dataPath + "/JsonData/spikes_VMDG_0.json";
			string jsonString = File.ReadAllText (path);
			SpikesData sd= JsonConvert.DeserializeObject<SpikesData>(jsonString);
			return sd;
		}
	}

	//classes used in SpikesData (can be internal), representing couples of basket/pyramid cells ids and timestamps
	public class I_Rec {
		public List<int> sp_src{get; set;}
		public List<float> sp_times{get; set;}

	}
	public class E_Rec {
		public List<int> sp_src{get; set;}
		public List<float> sp_times{get; set;}
	}

	//Fetch necessary data from program_VMDG_0
	public class ProgramData
	{
		//properties associated to the variables declared at the beginning (lines 65-67) through DataReader.SetValues()
		public string name;
		public float stop;
		public float start;

		/*
			this method reads program data from a json file and converts it to multiple attribute-value tuples, before returning it
			it is called in DataReader.Initiate(), after SpikesData.Initiate()
		*/
		public ProgramData[] Initiate(){
			string path = Application.dataPath + "/JsonData/program_VMDG_0.json";
			string jsonString = File.ReadAllText (path);
			ProgramData[] prd= JsonConvert.DeserializeObject<ProgramData[]>(jsonString);
			return prd;
		}
	}
}

