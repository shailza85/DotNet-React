import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';

export class Layout extends Component {
    static displayName = Layout.name;

    // This sets up the layout of the page. We'll have a nav menu at the top, and then this.props.children will be populated by React Router, causing the pages to have different bodies.
    render() {
        return (
            <div>
                <NavMenu />
                <Container>
                    {/* 
                     "this" points to the <Layout /> component. 
                     "props" points to the <Layout />'s properties.
                     "children" points the <Layout />'s nested "JSX/HTML" children.
                     */}
                    {this.props.children}
                </Container>
            </div>
        );
    }
}