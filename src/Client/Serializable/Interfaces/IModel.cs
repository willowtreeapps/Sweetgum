// <copyright file="IModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

namespace WillowTree.Sweetgum.Client.Serializable.Interfaces
{
    /// <summary>
    /// A model that can be converted to a serializable model.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <typeparam name="TSerializableModel">The serializable model type.</typeparam>
    public interface IModel<TModel, TSerializableModel>
        where TModel : IModel<TModel, TSerializableModel>
        where TSerializableModel : ISerializableModel<TSerializableModel, TModel>
    {
        /// <summary>
        /// Converts an instance of <typeparamref name="TModel"/> to an instance of <typeparamref name="TSerializableModel"/>.
        /// </summary>
        /// <returns>An instance of <typeparamref name="TSerializableModel"/>.</returns>
        TSerializableModel ToSerializable();
    }
}
