using System.Collections.Generic;
namespace JohnUtils
{
    public delegate void EventHandlerWithString(string word);
    public delegate void EventHandlerWithVoid();
    // Delegate for handling events with a List<template>
    public delegate void EventHandlerWithList<T>(List<T> items);
}
