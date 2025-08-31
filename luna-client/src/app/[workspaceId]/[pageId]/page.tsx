'use server';

import { Page } from "@/components/pages/page/Page";

export default async function pageContainer({ params }: { params: { workspaceId: string, pageId: string } }) {
	return <Page pageId={params.pageId}/>
}