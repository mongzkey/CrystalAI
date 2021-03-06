// GPL v3 License
// 
// Copyright (c) 2016-2017 Bismur Studios Ltd.
// Copyright (c) 2016-2017 Ioannis Giagkiozis
// 
// Option.cs is part of Crystal AI.
//  
// Crystal AI is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//  
// Crystal AI is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Crystal AI.  If not, see <http://www.gnu.org/licenses/>.
using System;


namespace Crystal {

  /// <summary>
  ///   Normally this option class should serve all the "option" related needs of an AI.
  /// </summary>
  public class Option : CompositeConsideration, IOption {
    IOptionCollection _collection;

    /// <summary>
    ///   The action to be executed when this option is selected.
    /// </summary>
    public IAction Action { get; private set; }

    /// <summary>
    /// Sets the action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    public bool SetAction(IAction action) {
      if(action == null)
        return false;

      Action = action;
      return true;
    }

    /// <summary>
    /// Sets the action to be executed when this option is selected to the action
    /// associated with actionId.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <returns>
    /// Returns true if the action was successfully set, false otherwise.
    /// </returns>
    public bool SetAction(string actionId) {
      if(string.IsNullOrEmpty(actionId))
        return false;
      if(_collection == null)
        return false;
      if(_collection.Actions.Contains(actionId) == false)
        return false;

      Action = _collection.Actions.Create(actionId);
      return true;
    }

    /// <summary>
    ///   Calculates the utility for this option given the provided context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The utility of this option.</returns>
    public override void Consider(IContext context) {
      if(Action.InCooldown)
        Utility = new Utility(0.0f, Weight);
      base.Consider(context);
    }

    /// <summary>
    /// Creates a new instance of the implementing class. Note that the semantics here
    /// are somewhat vague, however, by convention the "Prototype Pattern" uses a "Clone"
    /// function. Note that this may have very different semantics when compared with either
    /// shallow or deep cloning. When implementing this remember to include only the defining
    /// characteristics of the class and not its state!
    /// </summary>
    /// <returns></returns>
    public override IConsideration Clone() {
      return new Option(this);
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="T:Crystal.Option"/> class.
    /// </summary>
    public Option() {
      Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Crystal.Option"/> class.
    /// </summary>
    /// <param name="other">The other.</param>
    protected Option(Option other) : base(other) {
      _collection = other._collection;
      Weight = other.Weight;
      Measure = other.Measure?.Clone();
      Action = other.Action?.Clone();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Crystal.Option"/> class.
    /// </summary>
    /// <param name="nameId">The name identifier.</param>
    /// <param name="collection">The collection.</param>
    /// <exception cref="T:Crystal.Option.OptionAlreadyExistsInCollectionException"></exception>
    public Option(string nameId, IOptionCollection collection) : base(collection?.Considerations) {
      _collection = collection;
      NameId = nameId;
      Initialize();
      if(_collection.Add(this) == false)
        throw new OptionAlreadyExistsInCollectionException(nameId);
    }

    void Initialize() {
      Weight = 1.0f;
      Measure = new WeightedMetrics();
    }

    internal class OptionAlreadyExistsInCollectionException : Exception {
      string _message;

      public override string Message {
        get { return _message; }
      }

      public OptionAlreadyExistsInCollectionException(string nameId) {
        _message = string.Format("{0} already exists in options collection", nameId);
      }
    }
  }

}