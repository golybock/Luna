'use server';

import { Page } from "@/components/pages/page/Page";

export default async function pageContainer({ params }: { params: Promise<{ workspaceId: string, pageId: string }> }) {
	const { pageId } = await params;
	return <Page pageId={pageId}/>
}