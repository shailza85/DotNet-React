// File Summary: Main App class for React.

import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import { GetPeople } from './components/GetPeople';

import './custom.css'
import { CreatePerson } from './components/CreatePerson';
import { ModifyPerson } from './components/ModifyPerson';

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                {/* These React Router routes will pair a URL path to a component that is to be rendered. The component is identified by its class name. */}
                <Route exact path='/' component={Home} />
                <Route path='/counter' component={Counter} />
                <Route path='/fetch-data' component={FetchData} />
                <Route path='/admin/getpeople' component={GetPeople} />
                <Route path='/admin/createperson' component={CreatePerson} />
                <Route path='/admin/modifyperson' component={ModifyPerson} />


            </Layout>
        );
    }
}