using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PipeRun:IComparable<PipeRun>
{
    public GameObject go = null;

    public float MinRadius = float.MaxValue;
    public float MaxRadius = 0;
    public float RadiusDistance = 0;
    public PipeModelBase EndModel1;
    public PipeModelBase EndModel2;

    public override string ToString()
    {
        //return $"[{EndModel1.name}->{EndModel2.name}]({MaxRadius}-{MinRadius}={RadiusDistance})";
        return $"{MaxRadius}-{MinRadius}={RadiusDistance}";
    }

    public List<PipeModelBase> InitModels = new List<PipeModelBase>();

    public List<PipeModelBase> EndModels = new List<PipeModelBase>();

    public List<PipeModelBase> TeeModels = new List<PipeModelBase>();

    public List<PipeModelBase> PipeModels = new List<PipeModelBase>();
    public List<float> PipeRadiuses = new List<float>();

    //public PipeRun(List<PipeModelBase> endModels)
    //{
    //    InitModels.AddRange(endModels);
    //}

    public PipeRun(PipeModelBase startModel)
    {
        InitModels.Add(startModel);
        FindAllConnectedModels();
    }

    public void Init(PipeModelBase startModel)
    {
        InitModels.Add(startModel);
        FindAllConnectedModels();
    }

    public void FindAllConnectedModels()
    {
        List<PipeModelBase> currentEnds = new List<PipeModelBase>();
        currentEnds.AddRange(InitModels);
        int count = 0;
        int maxCount = 1000;
        while (currentEnds.Count >0 && count<maxCount)
        {
            count++;

            var model = currentEnds[0];
            currentEnds.RemoveAt(0);

            List<PipeModelBase> connectedModels = model.ConnectedModels;
            //if (!PipeModels.Contains(model))
            //{
            //    PipeModels.Add(model);
            //    if (connectedModels.Count <= 1 && !EndModels.Contains(model))
            //    {
            //        EndModels.Add(model);
            //    }
            //    if (connectedModels.Count >2 && !TeeModels.Contains(model))
            //    {
            //        TeeModels.Add(model);
            //    }
            //}

            AddModel(model);
            foreach (var newModel in connectedModels)
            {
                if (!currentEnds.Contains(newModel) && !PipeModels.Contains(newModel))
                {
                    currentEnds.Add(newModel);
                }
            }
        }
    }

    private void AddModel(PipeModelBase model)
    {
        List<PipeModelBase> connectedModels = model.ConnectedModels;
        if (!PipeModels.Contains(model))
        {
            PipeModels.Add(model);

            if(model.GetType()!=typeof(PipeReducerModel))
            {
                PipeRadiuses.Add(model.ModelStartPoint.w);
                PipeRadiuses.Add(model.ModelEndPoint.w);

                if (model.ModelStartPoint.w > MaxRadius)
                {
                    MaxRadius = model.ModelStartPoint.w;
                }
                if (model.ModelEndPoint.w > MaxRadius)
                {
                    MaxRadius = model.ModelEndPoint.w;
                }

                if (model.ModelStartPoint.w < MinRadius)
                {
                    MinRadius = model.ModelStartPoint.w;
                }
                if (model.ModelEndPoint.w < MinRadius)
                {
                    MinRadius = model.ModelEndPoint.w;
                }
            }

            if (connectedModels.Count <= 1 && !EndModels.Contains(model))
            {
                EndModels.Add(model);
            }
            if (connectedModels.Count > 2 && !TeeModels.Contains(model))
            {
                TeeModels.Add(model);
            }
        }
    }

    public PipeRun GetSortedRun()
    {
        if (EndModels.Count == 0)
        {
            Debug.Log($"SortFromEnd EndModels.Count == 0");
            return null;
        }
        PipeRun sortedRun = new PipeRun(EndModels[0]);
        sortedRun.EndModel1 = sortedRun.PipeModels[0];
        sortedRun.EndModel2 = sortedRun.PipeModels.Last();
        sortedRun.RadiusDistance = Mathf.Abs(sortedRun.MaxRadius - sortedRun.MinRadius);
        return sortedRun;
    }

    public int CompareTo(PipeRun other)
    {
        int r= other.RadiusDistance.CompareTo(this.RadiusDistance);
        if (r == 0)
        {
            r = other.PipeModels.Count.CompareTo(this.PipeModels.Count);
        }
        return r;
    }
}

[Serializable]
public class PipeRunList
{
    public List<PipeRun> PipeRuns = new List<PipeRun>();

    public List<PipeElbowModel> SpecialElbows = new List<PipeElbowModel>();

    public List<GameObject> PipeRunGos = new List<GameObject>();

    public PipeRunList()
    {

    }

    public PipeRunList(List<PipeModelBase> list, float minDis, bool isUniformRaidus)
    {
        DateTime start = DateTime.Now;
        List<PipeModelBase> models = new List<PipeModelBase>(list);
        
        for (int i = 0; i < models.Count; i++)
        {
            PipeModelBase model1 = models[i];
            model1.ConnectedModels.Clear();
        }

        for (int i=0;i<models.Count;i++)
        {
            PipeModelBase model1 = models[i];
            ProgressArg p1 = new ProgressArg("InitPipeRunList 1", i, models.Count, model1);
            if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
            {
                return;
            }
            for (int j=i+1;j<models.Count;j++)
            {
                ProgressArg p2 = new ProgressArg("InitPipeRunList 2", j- (i + 1), models.Count-(i + 1), model1);
                p1.AddSubProgress(p2);
                if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                {
                    return;
                }
                PipeModelBase model2 = models[j];
                PipeModelBase.IsConnected(model1, model2, minDis);
            }

            if (model1.ConnectedModels.Count == 0)
            {
                if (model1.GetType() == typeof(PipeElbowModel))
                {
                    //Debug.LogWarning($"Not Found Connected Elbow:{model1.name}");
                    PipeElbowModel elbow = model1 as PipeElbowModel;
                    for (int j = i + 1; j < models.Count; j++)
                    {
                        ProgressArg p2 = new ProgressArg("InitPipeRunList 3", j - (i + 1), models.Count - (i + 1), model1);
                        p1.AddSubProgress(p2);
                        if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                        {
                            return;
                        }
                        PipeModelBase model2 = models[j];
                        PipeModelBase.IsConnectedEx(elbow, model2, minDis, isUniformRaidus);
                    }

                    if (model1.ConnectedModels.Count == 0)
                    {
                        Debug.LogError($"Not Found Connected Elbow:{model1.name}");
                    }

                    SpecialElbows.Add(elbow);
                }
            }
        }

        int count = 0;
        int maxCount = 1000;
        while (models.Count > 0 && count < maxCount)
        {
            count++;
            PipeModelBase model1 = models[0];
            PipeRun run = new PipeRun(model1);
            foreach (var runModel in run.PipeModels)
            {
                models.Remove(runModel);
            }
            PipeRun run2 = run.GetSortedRun();
            PipeRuns.Add(run2);
        }

        PipeRuns.Sort();

        Debug.Log($"PipeRunList time:{DateTime.Now-start} models:{models.Count} minDis:{minDis} runs:{PipeRuns.Count}");

        ProgressBarHelper.ClearProgressBar();
    }


    public void RenameResultBySortedId()
    {
        for(int i=0;i<PipeRuns.Count;i++)
        {
            PipeRun run = PipeRuns[i];
            GameObject go = new GameObject($"PipeRun_{i:000}({run.PipeModels.Count})");
            PipeRunGos.Add(go);
            run.go = go;

            for (int j=0; j < run.PipeModels.Count;j++)
            {
                var resultObj = run.PipeModels[j].ResultGo;
                if (resultObj != null)
                {
                    if (j == 0)
                    {
                        go.transform.position = resultObj.transform.position;
                    }
                    resultObj.name = $"[{i:000}][{j:000}]_{resultObj.name}";
                    resultObj.transform.SetParent(go.transform);
                }
            }
        }
    }

}
