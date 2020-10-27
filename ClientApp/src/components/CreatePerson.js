import React, { Component } from 'react';
// Don't forget to "npm install axios" and import it on any pages from which you are making HTTP requests.
import axios from 'axios';

// The name of the class is used in routing in App.js. The name of the file is not important in that sense.
export class CreatePerson extends Component {
    static displayName = CreatePerson.name;

    constructor(props) {
        // 1) When we build the component, we assign the state to be loading, and register an empty list in which to store our forecasts.
        super(props);
        this.state = { statusCode: 0, response: [], firstName: "", lastName: "", phone: "", waiting: false };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        switch (event.target.id) {
            case "firstName":
                this.setState({ firstName: event.target.value });
                break;
            case "lastName":
                this.setState({ lastName: event.target.value });
                break;
            case "phone":
                this.setState({ phone: event.target.value });
                break;
        }

    }


    // Either way we render the title, and a description.
    render() {
        return (
            <div>
                <p>{this.state.waiting ? "Request sent, awaiting response." : "Response received, status: " + this.state.statusCode}</p>
                <p>Response Data: {JSON.stringify(this.state.response)}</p>
                <form onSubmit={this.handleSubmit}>
                    <label htmlfor="firstName">First Name:</label>
                    <input id="firstName" type="text" value={this.state.firstName} onChange={this.handleChange} />
                    <br />
                    <label htmlfor="lastName">Last Name:</label>
                    <input id="lastName" type="text" value={this.state.lastName} onChange={this.handleChange} />
                    <br />
                    <label htmlfor="phone">Phone:</label>
                    <input id="phone" type="text" value={this.state.phone} onChange={this.handleChange} />
                    <br />
                    <input type="submit" value="Submit" />
                </form>
            </div>
        );
    }


    async handleSubmit(event) {
        event.preventDefault();
        this.setState({ waiting: true });
        // Axios replaces fetch(), same concept. Send the response and "then" when it comes back, put it in the state.

        /*
        axios.post(`person/api/create?firstName=${this.state.firstName}&lastname=${this.state.lastName}&phone=${this.state.phone}`).then(res => {
            this.setState({ statusCode: res.status, response: res.data, loading: false });
        });
        */
        axios({
            method: 'post',
            url: 'person/api/create',
            params: {
                firstName: this.state.firstName,
                lastName: this.state.lastName,
                phone: this.state.phone
            }
        })
            .then((res) => {
                this.setState({ statusCode: res.status, response: res.data, waiting: false });
            })
            .catch((err) => {
                this.setState({ statusCode: err.response.status, response: err.response.data, waiting: false });
            });
    }
}