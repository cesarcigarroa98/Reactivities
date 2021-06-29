import { Redirect, Route, RouteComponentProps, RouteProps } from "react-router";
import { useStore } from "../stores/store";

interface Props extends RouteProps {
    component: React.ComponentType<RouteComponentProps<any>> | React.ComponentType<any>;
}

//Private route component helps to avoid user access URLs without being logged in

//Passing the rest of the properties available inside props and send it to the root component being used 
export default function PrivateRoute({component: Component, ...rest}: Props) {
    const {userStore: {isLoggedIn}} = useStore();

    //Redirect to home page
    return (
        <Route 
            {...rest}
            render={(props) => isLoggedIn ? <Component {...props} /> : <Redirect to='/'/>}
        />
    )

}