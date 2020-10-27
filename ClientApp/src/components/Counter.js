import React, { Component } from 'react';

export class Counter extends Component {
    static displayName = Counter.name;

    // Constructor runs first, builds the component. It sets the inital state and binds the incrementCounter method to the component.

    // 1) When the component is created, just like in C# OOP, the constructor builds the component. It sets the state (in this case a currentCount property, although there can be many state aspects), and binds any methods that are used as event listeners in the JSX.
    constructor(props) {
        super(props);
        // Set initial state.
        this.state = { currentCount: 0 };
        // Create the method bind for use in the onClick listener.
        this.incrementCounter = this.incrementCounter.bind(this);
    }


    // 3) This method gets called by the onClick event listener, which will change the state and trigger a re-render. 
    incrementCounter() {
        // By calling setState(), if it causes a state change, a re-render will take place.
        this.setState({
            currentCount: this.state.currentCount + 1
        });
    }

    // 2) Every time the component's state is changed using the setState() method, it will re-render the component to reflect that state change. This saves re-rendering the whole page if only one component has had its state changed.
    render() {
        return (
            <div>
                <h1>Counter</h1>

                <p>This is a simple example of a React component.</p>

                {/* Similarly to C# (Razor) we can output variables (state properties) into our JSX / HTML using braces. */}
                <p aria-live="polite">Current count: <strong>{this.state.currentCount}</strong></p>

                {/* Because we bound the method to the component, we can assign the method as an onClick listener to our JSX button. Without the bind it won't work. */}
                <button className="btn btn-primary" onClick={this.incrementCounter}>Increment</button>
            </div>
        );
    }
}