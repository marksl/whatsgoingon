
//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Model
{

using System;
    using System.Collections.Generic;
    
public partial class BuildData
{

    public BuildData()
    {

        this.BuildDiffs = new HashSet<BuildDiff>();

        this.BuildDiffs1 = new HashSet<BuildDiff>();

    }


    public int Id { get; set; }

    public int BuildId { get; set; }

    public string Category { get; set; }

    public string Type { get; set; }

    public string Name { get; set; }

    public string Data { get; set; }

    public bool Processed { get; set; }



    public virtual ICollection<BuildDiff> BuildDiffs { get; set; }

    public virtual ICollection<BuildDiff> BuildDiffs1 { get; set; }

    public virtual Build Build { get; set; }

}

}