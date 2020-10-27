import React, { Component } from 'react';
// Don't forget to "npm install axios" and import it on any pages from which you are making HTTP requests.
import axios from 'axios';

// The name of the class is used in routing in App.js. The name of the file is not important in that sense.
export class GetPeople extends Component {
    static displayName = GetPeople.name;

    constructor(props) {
        // 1) When we build the component, we assign the state to be loading, and register an empty list in which to store our forecasts.
        super(props);
        this.state = { people: [], loading: true };
    }

    componentDidMount() {
        // 2) When the component mounts, we make the async call to the server to retrieve the API results.
        this.populatePersonData();
    }

    static renderPersonTable(people) {
        // 5) When the async call comes back, render will call this method rather than rendering "Loading...", which will create our table.
        return (
            <div>

                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Full Name</th>
                            <th>Phone Numbers</th>
                        </tr>
                    </thead>
                    <tbody>
                        {/* Note that we have "forecast.date" twice. That is because the "key=" attribute is for identifying a row if a "edit" or "delete", etc. button is present. It is not for displaying data. If you want to display that data point, you will need it inside of a <td> as well.*/}
                        {people.map(person =>
                            <tr key={person.id}>
                                <td>{person.id}</td>
                                <td>{person.fullName}</td>
                                <td>{person.phoneNumbers.join(', ')}</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    render() {
        // 4) When we render, this ternary statement will with print loading, or render the forecasts table depending if the async call has come back yet.
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : GetPeople.renderPersonTable(this.state.people);

        // Either way we render the title, and a description.
        return (
            <div>
                <h1 id="tabelLabel" >People</h1>
                <p>This component demonstrates fetching data from the server.</p>
                {contents}
            </div>
        );
    }

    async populatePersonData() {
        // 3) Make the async call to the API.
        // When an async call is made, it "awaits" a response. This means that rather than the server hanging and keeping the "thread" (process) open, it shelves the thread to be picked up when the response comes back.
        // This frees up server resources to do other things in the event the request takes a few seconds (or more, if your internet is straight out of 1995).

        // Axios replaces fetch(), same concept. Send the response and "then" when it comes back, put it in the state.
        axios.get('person/api/all').then(res => {
            this.setState({ people: res.data, loading: false });
        });
    }
}